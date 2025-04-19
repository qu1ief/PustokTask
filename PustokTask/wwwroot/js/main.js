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

    $('.addtobasket').click(function (e){
        e.preventDefault();

        let url = $(this).attr('href');

        alert(url)
            fetch(url)
            .then(response => response.text())
            .then(data => {
                $(".cart-dropdown-block").html(data);
          
            });
    }
});
