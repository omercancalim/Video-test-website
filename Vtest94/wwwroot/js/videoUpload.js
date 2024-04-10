
document.getElementById('VideoFile').addEventListener('change', function (e) {
    if (e.target.files.length > 0) {
        var file = e.target.files[0];
        var url = URL.createObjectURL(file);
        var video = document.getElementById('videoPreview');
        var slider = document.getElementById('frameSlider');

        video.src = url;
        video.addEventListener('loadedmetadata', function () {
            // Set the slider range based on video duration
            slider.min = 0;
            slider.max = video.duration;
            slider.value = 0; // Reset slider to start position

            // Show the slider and any other UI elements here
            document.getElementById('thumbnailSelector').style.display = 'block';
            document.getElementById('videoPrevContainer').style.display = 'block';
        });
    }
});


document.getElementById('frameSlider').addEventListener('input', function () {
    var video = document.getElementById('videoPreview');
    var sliderValue = this.value;
    document.getElementById('ThumbnailFrameTime').value = sliderValue; // Update hidden field value
    if (video.src) {
        video.currentTime = this.value;
        // Optionally, update a frame preview here if separate from the video element
    }
});