$(document).ready(function () {
  $("#load-more").click(function () {
    var button = $(this);
    $.ajax({
      url: "/account/items?skip=" + button.attr("data-skip"),
      method: "get",
      success: function (data) {
        if (data.length) {
          $(data).insertBefore(button);
          button.attr("data-skip", parseInt(button.attr("data-skip")) + 1);
        }
      },
      error: function(err) {
        console.log(err.responseText);
      }
    });
  });

  $('.items-wrapper').on('click', '.delete-item', function () {
    if (!$('.confirm-form-container').length) {
      let target = $(this);
      let itemName = target.parent().prev().find('.item-title').html();
      let itemId = target.attr("data-for-id");
      confirmDelete(itemName, itemId);
    }
  });

  $('.items-wrapper').on('click', '.see-item', function () {
    window.location.href = "/items/details/" + $(this).attr('data-id');
  });

  $('.items-wrapper').on('click', '.edit-item', function () {
    window.location.href = "/items/edit/" + $(this).attr('data-id');
  });

  var timer;
  $('.items-wrapper').on('click', '.item', function () {
    if(window.innerWidth <= 870) {
      $('.overlay.active').removeClass("active");
      $(this).find('.overlay').addClass('active');
      var _this = $(this);

      hidder();
    }
  });

  function hidder() {
    clearTimeout(timer);
    timer = setTimeout(function () {
      if($('.overlay.active').length)
        $('.overlay.active').css({
          top: "-100px",
          opacity: 0
        });
        setTimeout(function () {
          var a = $('.overlay.active');
          a.removeClass('active');
          a.removeAttr("style");
        }, 200);
    }, 3500);
  }

  function confirmDelete(item_name, item_id) {
    var container = $("<div>").addClass("confirm-form-container");
    var heading = $("<div>").addClass("form-heading");
    var buttons = $("<div>").addClass("form-buttons");
    var confirm = $("<div>").addClass("confirm-button");
    var cancel = $("<div>").addClass("cancel-button");

    heading.html("Потвърждавате ли изтриването на изделие <span>" + item_name + "</span>");
    confirm.html("изтрий");
    cancel.html("откажи");

    confirm.click(function () {
      _deleteItem(item_id);
    });

    cancel.click(function () {
      hideConfirmationForm();
    });

    buttons.append(confirm);
    buttons.append(cancel);
    container.append(heading);
    container.append(buttons);

    container.css("transform", "translateY(-50%) scale(0.01)");
    $('body').append(container);
    setTimeout(function () {
      $(".confirm-form-container").removeAttr("style");
    }, 100);
  }

  function _deleteItem(itemId) {
    $.ajax({
      method: "post",
      url: "/items/delete/" + itemId,
      success: function(data) {
        if(data.Data == "success") {
          var item = $('.item[data-id="' + itemId + '"]');
          hideConfirmationForm();
          setTimeout(function () {
            item.css("height", "0px");
          }, 500);
          setTimeout(function () {
            item.remove();
          }, 700);
        }
      },
      error: function(err) {
        console.log(err.responseText);
      }
    });
  }

  function hideConfirmationForm() {
    $('.confirm-form-container').css("transform", "translateY(-50%) scale(0.01)");
    setTimeout(function () {
      $('.confirm-form-container').remove();
    }, 300);
  }
});
