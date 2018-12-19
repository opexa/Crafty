$(document).ready(function () {
  $('.profile-image').hover(function () {
    $('.edit-banner-btn').css("height", "0px");
  },
  function () {
    $('.edit-banner-btn').removeAttr("style");
  });

  $('.fa.fa-edit').click(function () {
    $('input[name="profile_img"]').trigger('click');
  });

  $('.fa.fa-file-photo-o').click(function (ev) {
    $('input[name="banner_img"]').trigger('click');
  });

  $('input[name="profile_img"]').change(function () {
    readURL(this);
  });

  $('input[name="banner_img"]').change(function () {
    readURL(this);
  });

  function readURL(input) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = function (e) {
        if(input.getAttribute("name") === "profile_img")
          $(".profile-image").find('img').attr("src", e.target.result);
        else if(input.getAttribute("name") === "banner_img")
          $(".banner-image").attr("style", "background-image: url('" + e.target.result + "')");
      }

      reader.readAsDataURL(input.files[0]);
    }
  }
  // Changing profile status: DISABLED
  // $('.choose-status-btn').click(function () {
  //   if(!$('.status-dialog-wrapper').length)
  //     createStatusDialog();
  // });

  // $(document).click(function (ev) {
  //   if(ev.target.classList.contains("choose-status-btn")) {
  //     return;
  //   }
  //
  //   if($('.status-dialog-wrapper').length) {
  //     if(!ev.target.classList.contains("status-option")
  //          && !ev.target.classList.contains("user-status-info")
  //          && !ev.target.classList.contains("seller-status-info")) {
  //       $('.status-dialog-wrapper').addClass('bald');
  //       setTimeout(function () {
  //         $('.status-dialog-wrapper').remove();
  //       }, 500);
  //    }
  //   }
  // });
  //
  // function createStatusDialog() {
  //   let wrapper = $("<div>").addClass("status-dialog-wrapper bald");
  //   let container = $("<div>").addClass("dialog-container");
  //   let userStatus = $("<div>").addClass("status-option user-status-option");
  //   let sellerStatus = $("<div>").addClass("status-option seller-status-option");
  //   let userStatusIcon = $("<i>").addClass("fa fa-user-o");
  //   let sellerStatusIcon = $("<i>").addClass("fa fa-user-plus");
  //   let userStatusName = $("<span>").addClass("status-name").html("Потребител");
  //   let selelrStatusName = $("<span>").addClass("status-name").html("Продавач");
  //
  //   let userStatusInfo = $("<div>").addClass("user-status-info")
  //     .html("<span>Ако искате само да пазарувате от Crafty.bg, то това е статусът, който трябва да изберете</span>");
  //
  //   let sellerStatusInfo = $("<div>").addClass("seller-status-info")
  //     .html("<span>Ако вие ще ползвате Crafty.bg с цел разпространяване и предлагане на вашият труд и изкуство, то вие следва да изберете статус продавач</span>");
  //
  //   userStatus.append(userStatusIcon).append(userStatusName).append(userStatusInfo).append($("<div>").addClass("left-triangle"));
  //   sellerStatus.append(sellerStatusIcon).append(selelrStatusName).append(sellerStatusInfo).append($("<div>").addClass("right-triangle"));
  //
  //   userStatus.click(function () {
  //     $("#Status").val("1");
  //     $('.choose-status-btn').html("Потребител");
  //   });
  //
  //   sellerStatus.click(function () {
  //     $("#Status").val("0");
  //     $('.choose-status-btn').html("Продавач");
  //   });
  //
  //   container.append(userStatus).append(sellerStatus);
  //   wrapper.append(container);
  //
  //   $('body').append(wrapper);
  //   setTimeout(function () {
  //     $('.status-dialog-wrapper').removeClass("bald");
  //   }, 100);
  // }
});
