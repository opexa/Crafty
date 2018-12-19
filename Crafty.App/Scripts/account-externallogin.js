$(document).ready(function () {
  $("#submit-form").click(function (ev) {
    ev.preventDefault();
    $('.fa.fa-times-circle').addClass("invisible");
    $('.fa.fa-check-circle').addClass("invisible");
    $('.fa.fa-spinner.fa-pulse').removeClass("invisible");

    let usernameVal = $("#Username").val();

    $.ajax({
      url: "/account/checkname/" + usernameVal,
      method: "Get",
      success: function(data) {
        if(data === "free") {
          $('.fa.fa-spinner.fa-pulse').addClass("invisible");
          $('.fa.fa-times-circle').addClass("invisible");
          $('.fa-check-circle.fa').removeClass("invisible");
          $("#submit-form").addClass("invisible");
          $(".username-taken-error").addClass("invisible");
          setTimeout(function () {
            $(".form-wrapper").find('form.form-horizontal').submit();
          }, 1000);
        } else {
            $(".fa.fa-spinner.fa-pulse").addClass("invisible");
            $('.fa.fa-times-circle').removeClass("invisible");
            $(".username-taken-error").removeClass("invisible");
        }
      },
      error: function (data) {
        console.log(data);
      }
    })
  });
});
