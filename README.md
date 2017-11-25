# SpotifyController

<h1>Original Propblem:</h1>
You host a party use spotify for the music. You create yourself a playlist and play it.<br/>
Yout tell everybody to add song which they want to play to the queue. <br/>
Some idiot doubleclicks a song and the playlist context is replaced by some weird search result.<br/>
<br/>
Solution: <br/>
A small application that allows you to controll spotify. <br/>
It has its own "session"-context.<br/>
The application has its own additional queue so that people can add their songs manually.<br/>
But no matter what they do as soon as all items from the manual queue are played, the application will proceed with tracks from the session.<br/>


<h1>Additional Problems:</h1>
Creating a playlist for a party is painful. Whould be useful if you could just base your playlist on other users playlists.

<h1>Idea:</h1>
Create a search that retrieves a amount of playlist based on a search.<br/>
Count the occurence of each track through all playlists<br/>
Somehow create a playlist based on that result.
