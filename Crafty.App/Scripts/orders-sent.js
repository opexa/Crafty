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

    $('.action.order-finished').click(function () {
      let orderId = $(this).attr("data-order-id");
      markOrderAsFinished(orderId);
    });

    $('.orders-container').on('click', '.expand-order', function () {
      let btn = $(this);

      let order = btn.parent();
      order.css({
        "height": "auto",
        "max-height": "600px"
      });

      replaceExpandBtn(btn, "open");
    });

    $('.orders-container').on('click', '.collapse-order', function () {
      let btn = $(this);
      let order = btn.parent();

      order.removeAttr("style");

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
  function markOrderAsFinished(id) {
    $.ajax({
      url: "/orders/orderfinished?o=" + id,
      method: "post",
      success: function (data) {
        if(data === "success") {
          releaseOrder(id);
        } else {
            occuredError(id);
        }
      },
      error: function (err) {
        occuredError(id);
        console.log(err);
      }
    })
  }

  function occuredError(orderId) {
    hideOrderDetails();
    let order = $(".order[data-id='" + orderId + "']");
    let errorDiv = $('<div class="order-error" data-id="' + orderId + '" style="height: 0px">');
    // order.find('.overlay').remove();
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
