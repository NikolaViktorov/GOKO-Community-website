let isVideoPlaying = true;
let vid = document.getElementById("video-background");

vid.addEventListener("click", playVid);

function playVid() {
    if (isVideoPlaying) {
        vid.pause();
        isVideoPlaying = false;
    } else {
        vid.play();
        isVideoPlaying = true;
    }
}
