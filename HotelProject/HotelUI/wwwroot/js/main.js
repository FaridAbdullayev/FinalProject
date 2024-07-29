$(".delete-btn").click(function (e) {
    e.preventDefault();

    let url = $(this).attr("href");


    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(url)
                .then(response => {
                    if (response.ok) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Your file has been deleted.",
                            icon: "success"
                        }).then(() => {
                            window.location.reload();
                        })
                    }
                    else if (response.status == 404) {
                        Swal.fire({
                            title: "Sorry!",
                            text: "Data not found",
                            icon: "error"
                        })
                    }
                    else if (response.status == 401) {
                        Swal.fire({
                            title: "Sorry!",
                            text: "Data not found",
                            icon: "error"
                        }).then(() => window.location = "/account/login")
                        
                    }
                    else {
                        Swal.fire({
                            title: "Sorry!",
                            text: "Something went wrong",
                            icon: "error"
                        })
                    }
                })
        }
    });


    
})

$(document).ready(function () {
    $(".imgInput").change(function (e) {
        let box = $(this).parent().find(".preview-box");
        $(box).find(".previewImg").remove();

        for (var i = 0; i < e.target.files.length; i++) {

            let img = document.createElement("img");
            img.style.width = "200px";
            img.classList.add("previewImg");

            let reader = new FileReader();
            console.log(e.target.nextElementSibling);
            reader.readAsDataURL(e.target.files[i]);
            reader.onload = () => {
                img.setAttribute("src", reader.result);
                $(box).append(img)
            }
        }

    })

    $(".remove-img-icon").click(function () {
        $(this).parent().remove();
    })
});