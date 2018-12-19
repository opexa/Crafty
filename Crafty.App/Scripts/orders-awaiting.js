$(document).ready(function () {
  (function attachBodyEventListeners() {
    $('.hoverable').find('a').hover(function () {
        var a = $(this);
        a.parent().find('.triangle').css("opacity", 1)
        a.parent().find('.popup').css("opacity", 1);
      },
      function () {
        var a = $(this);
        a.parent().find('.triangle').removeAttr('style');
        a.parent().find('.popup').removeAttr('style');
    });

    $('.orders-container').on('click', '.order-options', function (ev) {
      ev.stopPropagation();

      if($('.order-options.active').length) {
        $('.order-options.active').find(".order-options-menu").removeAttr("style");
        $('.order-options.active').removeClass("active");
      }

      var toggle = $(this);
      var menu = toggle.find('.order-options-menu');

      toggle.addClass("active");
      var height = menu.children().length * 39;
      menu.css("height", height + "px");
    });

    $('.orders-container').on('click', '.order-options.active', function () {
      var toggle = $(this);
      var menu = toggle.find('.order-options-menu');
      menu.removeAttr("style");
      toggle.removeClass("active");
    });

    $('body').on('click', function (ev) {
      // ev.stopPropagation();
      var target = $(ev.target);
      // console.log(target.hasClass("track-num-input"));

      if((!target.hasClass("option") || !target.hasClass("order-options")) && $('.order-options.active').length) {
        $('.order-options.active').find(".order-options-menu").removeAttr("style");
        $('.order-options.active').removeClass("active");
      }
      //
      // if($('#order-track-num').length
      // && $('.action.order-sent.active').has(target).length === 0
      // && !$('.action.order-sent.active').is(target)) {
      //   // hideTrackInput();
      // }
    });

    $('.order-sent').click(function () {
      var tag = $(this);
      if(!tag.find('input').length) {
        let html = tag.html();
        tag.addClass("active");

        let trackNumInput = $('<input type="text">').attr({
          "id": "order-track-num",
          "class": "track-num-input",
          "placeholder": "товарителница №"
        });
        trackNumInput.on('keyup', function (ev) {
          var input = $(this);
          if(input.val().length > 0) {
            input.next().addClass("fa-paper-plane-o");
            input.next().removeClass("fa-close");
          } else {
            input.next().addClass("fa-close");
            input.next().removeClass("fa-paper-plane-o");
          }

          if(ev.keyCode == 13 && input.val().length > 0) {
            let orderId = input.parent().attr("data-order-id");
            markOrderAsSent(orderId);
          } else if (ev.keyCode == 27) {
            hideTrackInput();
          }
        });

        let icon = $('<i>').addClass("fa fa-close").attr("id", "submit-order-sent");
        tag.html("");

        setTimeout(function () {
          tag.append(trackNumInput).append(" ").append(icon);
          tag.css("width", "200px");
          trackNumInput.focus();
        }, 500);
        tag.data("html", html);
      }
    });

    $('.order-sent').on('click', '.fa-paper-plane-o', function (ev) {
      let orderId = $(this).parent().attr("data-order-id");
      markOrderAsSent(orderId);
    });

    $('.order-sent').on('click', '.fa-close', function () {
      hideTrackInput();
    })

    $('.orders-container').on('click', '.expand-order', function () {
      let btn = $(this);

      let order = btn.parent();
      order.css({
        "height": "auto",
        "max-height": "600px"
      });
      order.addClass("expanded");

      replaceExpandBtn(btn, "open");
    });

    $('.orders-container').on('click', '.collapse-order', function () {
      let btn = $(this);
      let order = btn.parent();

      order.removeAttr("style");
      order.removeClass("expandend");

      replaceExpandBtn(btn, "close");
    });
  }());

  function replaceExpandBtn(button, to) {
    if(to === "open") {
      button.addClass("collapse-order");
      button.find('i.fa.fa-chevron-down').removeClass("fa-chevron-down").addClass("fa-chevron-up");
    } else if (to === "close") {
        button.removeClass("collapse-order");
        button.find('i.fa.fa-chevron-up').removeClass("fa-chevron-up").addClass("fa-chevron-down");
    }
  }

  function hideTrackInput() {
    var tag = $("#order-track-num").parent();
    tag.removeAttr("style");
    setTimeout(function () {
      tag.html(tag.data("html"));
      tag.removeData("html");
      tag.removeClass("active");
    }, 400);
  }

  function markOrderAsSent(orderId) {
    let order = $(".order[data-id='" + orderId + "']");
    let overlay = $('<div class="overlay" data-for-id="' + orderId + '">');
    let icon = $('<i class="fa fa-cog fa-spin fa-fw margin-bottom"></i>');

    overlay.append(icon);
    order.append(overlay);

    $.ajax({
      url: "/orders/statSent?o=" + orderId + "&tn=" + order.find("#order-track-num").val(),
      method: "post",
      success: function (data) {
        if(data == "success")
          releaseOrder(orderId);
        else if(data == "error")
          occuredError(orderId);
        else
          occuredError(orderId);
      },
      error: function (err) {
        console.log(err.responseText);
      }
    });
  }

  function occuredError(orderId) {
    let order = $(".order[data-id='" + orderId + "']");
    let errorDiv = $('<div class="order-error" data-id="' + orderId + '" style="height: 0px">');
    order.find('.overlay').remove();
    order.find('.order-error').remove();

    errorDiv.html("Възникна грешка.Моля опитайте отново.");
    order.prepend(errorDiv);
    setTimeout(function () {
      errorDiv.css("height", "35px");
    }, 200);
  }

  function releaseOrder(orderId) {
    let order = $(".order[data-id='" + orderId + "']");
    order.css({
      "min-height": "0px",
      height: "50px"
    });
    order.find('.overlay').remove();

    setTimeout(function () {
      order.css({
        "transform": "translateX(-1500px)",
        "opacity": "0",
        height: "0px"
      });
    }, 400);

    setTimeout(function () {
      order.remove();

      let ordersCount = $('.orders-count');
      let awaitOrdersCount = parseInt(ordersCount.html());

      if(awaitOrdersCount == 1) {
        let icon = $('<i class="fa fa-hourglass-end">');
        ordersCount.parent().html(icon).append(" Чакащи");
      } else {
          ordersCount.html(awaitOrdersCount - 1);
      }
    }, 800);
  }
});
