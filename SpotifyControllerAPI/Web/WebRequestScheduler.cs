using RandomUtilities.Queue;
using SpotifyControllerAPI.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Web
{
    public class WebRequestScheduler
    {
        public event EventHandler<WebRequestStatus> StatusChanged;

        public const int LOCKDOWN_WAITTIME_MULTIPLIER = 3;

        private const int THREADSLEEPTIME = 100;

        private const int LOCKDOWNCAUTIONREQUESTS = 15;

        private const int MAX_PARALLEL_TASKS = 16;

        public static WebRequestScheduler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WebRequestScheduler();
                return _instance;
            }
        }

        private static WebRequestScheduler _instance;

        private ConcurrentDictionary<string, bool> _threadHearts = new ConcurrentDictionary<string, bool>();

        private FILO<string> _tokenQueue = new FILO<string>();

        private ConcurrentDictionary<string, HttpWebRequest> _requestStorage = new ConcurrentDictionary<string, HttpWebRequest>();

        private ConcurrentDictionary<string, SchedulerResult> _resultStorage = new ConcurrentDictionary<string, SchedulerResult>();

        private WebRequestScheduler()
        {
            WatcherThread();
        }

        private bool _onLockDown = false;
        private int _lockDownWaitTime = 0;
        private long _lockDownTimestamp = 0;
        private int _lockdownCautionRequests = 0;



        private void WatcherThread()
        {
            ServicePointManager.DefaultConnectionLimit = MAX_PARALLEL_TASKS;

            _lockDownTimestamp = Environment.TickCount;

            FireStatusChanged();

            Task.Run(() =>
            {

                while (true)
                {
                    if(!_onLockDown)
                    {
                        if ((_threadHearts.Count() < 1 || (_tokenQueue.Count() > 5 * _threadHearts.Count && (_lockDownWaitTime * 1000 * LOCKDOWNCAUTIONREQUESTS) + _lockDownTimestamp < Environment.TickCount)))
                        {
                            if (_threadHearts.Count < MAX_PARALLEL_TASKS)
                            {

                                string threadGuid = Guid.NewGuid().ToString();

                                _threadHearts.TryAdd(threadGuid, true);

                                Task.Run(() => RunThread(threadGuid));

                            }
                        }
                    }
                    else
                    {
                        if (_threadHearts.Count == 0 && Environment.TickCount > _lockDownTimestamp + _lockDownWaitTime * LOCKDOWN_WAITTIME_MULTIPLIER) // 2000 + _lockDownWaitTime * 1000  < (Environment.TickCount - _lockDownTimestamp))
                        {
                            _onLockDown = false;
                        }
                    }

                    Thread.Sleep(THREADSLEEPTIME * 2);
                    FireStatusChanged();
                }
            });
        }
        

        private async void RunThread(string threadGuid)
        {
            try
            {
                int waitCount = 0;

                while (_threadHearts[threadGuid] && !_onLockDown)
                {
                    if (_tokenQueue.HasItems && !_onLockDown) // && _lockdownCautionRequests * 1000 * _lockDownWaitTime + _lockDownTimestamp < Environment.TickCount)
                    {
                        string token = _tokenQueue.Pull();

                        waitCount = 0;


                        SchedulerResult result = null;

                        try
                        {
                            try
                            {
                                //if (_lockdownCautionRequests < LOCKDOWNCAUTIONREQUESTS)
                                //    _lockdownCautionRequests++;

                                _requestStorage.TryGetValue(token, out HttpWebRequest request);

                                result = new SchedulerResult((HttpWebResponse)await request.GetResponseAsync());
                            }
                            catch (WebException webEx)
                            {
                                if (webEx.Response is HttpWebResponse res && (int)res.StatusCode == 429)
                                {
                                    _onLockDown = true;
                                    _lockDownTimestamp = Environment.TickCount;

                                    _lockdownCautionRequests = 1;

                                    int.TryParse(webEx.Response.Headers["Retry-After"], out _lockDownWaitTime);

                                    _tokenQueue.Push(token);
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new SchedulerResult(ex);
                        }

                        if (result != null)
                        {
                            _requestStorage.TryRemove(token, out HttpWebRequest req);
                            _resultStorage.TryAdd(token, result);
                        }
                    }
                    else
                    {
                        if (waitCount > 5 && _threadHearts.Count > 1)
                            _threadHearts[threadGuid] = false;

                        waitCount++;
                        Thread.Sleep(THREADSLEEPTIME);
                    }
                }
            }
            catch { }

            _threadHearts.TryRemove(threadGuid, out bool bla);
        }


        public async Task<SchedulerResult> RunInSchedule(HttpWebRequest request)
        {
            string token = Guid.NewGuid().ToString();

            _requestStorage.TryAdd(token, request);

            _tokenQueue.Push(token);

            return await Task.Run(() => GetResponse(token));
        }

        private SchedulerResult GetResponse(string token)
        {
            while (!_resultStorage.ContainsKey(token))
            {
                Thread.Sleep(10);
            }

            SchedulerResult result = null;

            _resultStorage.TryRemove(token, out result);

            return result;
        }

        private void FireStatusChanged()
        {
            WebRequestStatus eventArgs = new WebRequestStatus()
            {
                Blocked = _onLockDown,
                RemainingWaitTime = (LOCKDOWN_WAITTIME_MULTIPLIER * _lockDownWaitTime) - (int)(Environment.TickCount - _lockDownTimestamp),
                RequestsInQueue = _tokenQueue.Count(),
                RunningThreads = _threadHearts.Count
            };

            StatusChanged?.Invoke(this, eventArgs);
        }
    }
}
