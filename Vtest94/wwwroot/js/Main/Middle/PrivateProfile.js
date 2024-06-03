$(document).ready(function () {

    // Check for the 'popup' parameter in the URL
    var urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('popup') === 'true') {
        showPopup();

        var url = new URL(window.location.href);
        url.searchParams.delete('popup');
        window.history.replaceState({}, document.title, url.toString());
    }
    document.getElementById('uploadPhotoBtn').addEventListener('click', function () {
        $('#uploadPhotoModal').modal('show');
    });

    var cropper;
    
    document.getElementById('fileInput').addEventListener('change', function (e) {
        var file = e.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (event) {
                var imgElement = document.createElement('img');
                document.getElementById('imageToCrop').src = event.target.result;

                $('#uploadPhotoModal').modal('hide');
                $('#cropPhotoModal').modal('show');

                cropper = new Cropper(document.getElementById('imageToCrop'), {
                    aspectRatio: 1,
                    viewMode: 1,
                    dragMode: 'crop',
                    autoCropArea: 0.8,
                    responsive: true,
                    background: true,
                    modal: true,
                    guides: true,
                    highlight: true,
                    cropBoxMovable: true,
                    cropBoxResizable: true,
                    toggleDragModeOnDblclick: true,
                    minContainerWidth: 300,
                    minContainerHeight: 300,
                    maxCanvasWidth: 600,
                    maxCanvasHeight: 600 / (16 / 9)
                });
            }
            reader.readAsDataURL(file);
        }
    });

    document.getElementById('saveCroppedPhotoBtn').addEventListener('click', function () {
        var canvas = cropper.getCroppedCanvas();
        canvas.toBlob(function (blob) {
            document.getElementById('profileImage').src = URL.createObjectURL(blob);
            $('#cropPhotoModal').modal('hide');
            $('#uploadPhotoModal').modal('show');

            document.getElementById('savePhotoBtn').blob = blob;
        });
    });

    document.getElementById('savePhotoBtn').addEventListener('click', async function () {
        var newUsername = document.getElementById('usernameInput').value;
        var usernameError = document.getElementById('usernameError');

        // Client-side validation
        if (!newUsername) {
            usernameError.style.display = 'inline';
            return;
        } else {
            usernameError.style.display = 'none';
        }

        var blob = this.blob;

        try {
            const response = await fetch(`/api/user/update?newArbUsername=${newUsername}`);
            const data = await response.json();
            if (data.success) {
                // console.log('Username updated successfully.');
                if (!blob) {
                    var url = new URL(window.location.href);
                    url.searchParams.set('popup', 'true');
                    window.location.href = url.toString();
                }
            } else {
                alert('Failed to update username.');
            }
        } catch (error) {
            console.error('Error updating username:', error);
        }

        // Upload photo if a new one is selected
        if (blob) {
            var formData = new FormData();
            formData.append('file', blob, 'profile.jpg');

            fetch('/User/UploadPhoto', {
                method: 'POST',
                body: formData
            }).then(response => response.json())
                .then(data => {
                    if (data.success) {
                        console.log('Photo uploaded successfully.');
                        var url = new URL(window.location.href);
                        url.searchParams.set('popup', 'true');
                        window.location.href = url.toString();
                    } else {
                        alert('Failed to upload photo.');
                    }
                }).catch(error => {
                    console.error('Error uploading photo:', error);
                });
        }
    });

    // Clear file input when modals are closed
    $('#uploadPhotoModal').on('hide.bs.modal', function () {
        document.getElementById('fileInput').value = '';
        document.getElementById('profileImage').src = defaultProfileImageSrc;
    });

    $('#cropPhotoModal').on('hide.bs.modal', function () {
        document.getElementById('fileInput').value = '';
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
    });
});

function showPopup(callback) {
    var popup = document.getElementById('popupInfoBox');
    popup.style.display = 'block';
    setTimeout(function () {
        popup.style.opacity = '1';
    }, 10); // Slight delay to trigger the CSS transition

    // Hide the popup after 2 seconds
    setTimeout(function () {
        popup.style.opacity = '0';
        setTimeout(function () {
            popup.style.display = 'none';
            if (callback) {
                callback();
            }
        }, 500); // Wait for the opacity transition to finish before hiding
    }, 2000);
}