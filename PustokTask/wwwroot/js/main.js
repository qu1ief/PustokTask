$(document).ready(function () {
    $('.bookModal').click(function (e) {
        e.preventDefault();
        let url = $(this).attr('href'); 
        fetch(url)
            .then(response => response.text())
            .then(data => {
                $('#quickModal .modal-dialog').html(data);
            });
        $('#quickModal').modal('show');
    });

   
});

$(document).on('click', '.addtobasket', function (e) {
    e.preventDefault(); 

    let url = $(this).attr('href');

    fetch(url)
        .then(response => response.text())
        .then(data => {
            $(".cart-dropdown-block").html(data);
        })
        .catch(error => console.error("Xəta:", error));
});
