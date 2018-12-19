$("document").ready(function () {

  $(document).on('click', function (ev) {
    var target = $(ev.target);

    if((!target.hasClass("option") || !target.hasClass("purchase-options")) && $('.purchase-options.active').length) {
      $('.purchase-options.active').find(".purchase-options-menu").removeAttr("style");
      $('.purchase-options.active').removeClass("active");
    }

    if($(".body-overlay").length && !target.hasClass('popup-container') && target.parents('.popup-container').length < 1) {
      return closeOrderCancelConfirmationForm();
    }

    if($('.feedback-overlay').length && !$(".feedback-window").is(target) && $(".feedback-window").has(target).length === 0) {
      return hideFeedbackForm();
    }

    if($('.overlay').length && !$('.details-container').is(target) && $('.details-container').has(target).length === 0) {
      return hideOrderDetails();
    }
  });

  function showOrderDetails(model) {
    let overlay = $("<div class='overlay'>");

    overlay.css("opacity", "0");

    overlay.append(model);
    overlay.find('.details-container').css({
      "height": "0px",
      "padding": "0px"
    });
    overlay.appendTo($('body'));

    $('.details-container-content').perfectScrollbar();

    setTimeout(function () {
      $('.overlay').removeAttr("style");
    }, 300);

    setTimeout(function () {
      $('.overlay').find('.details-container').removeAttr("style");
    }, 900);


    overlay.find('.hoverable').find('a').hover(function () {
      var a = $(this);
      a.parent().find('.triangle').css("opacity", 1)
      a.parent().find('.popup').css("opacity", 1);
    },
    function () {
      var a = $(this);
      a.parent().find('.triangle').removeAttr('style');
      a.parent().find('.popup').removeAttr('style');
    });
  }

  function hideOrderDetails() {
    $('.overlay').find('.details-container').css({
      "height": "0px",
      "padding": "0px"
    });

    setTimeout(function () {
      $('.overlay').css("opacity", "0");
    }, 800);

    setTimeout(function () {
      $('.overlay').remove();
    }, 1200)
  }

  (function attachPurchaseClickEvents() {
    // $('.item-title').find('a').hover(function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').css("opacity", 1)
    //     a.parent().find('.popup').css("opacity", 1);
    //   },
    //   function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').removeAttr('style');
    //     a.parent().find('.popup').removeAttr('style');
    // });

    // $('.username-container').find('a').hover(function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').css("opacity", 1)
    //     a.parent().find('.popup').css("opacity", 1);
    //   },
    //   function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').removeAttr('style');
    //     a.parent().find('.popup').removeAttr('style');
    // });

    $('.cancel-order-btn').click(function () {
      let orderId = $(this).attr("data-order-id");
      cancelOrderConfirmation(orderId);
    });


    $('.purchases-container').on('click', '.purchase-options', function (ev) {
      ev.stopPropagation();

      if($('.purchase-options.active').length) {
        $('.purchase-options.active').find(".purchase-options-menu").removeAttr("style");
        $('.purchase-options.active').removeClass("active");
      }

      var toggle = $(this);
      var menu = toggle.find('.purchase-options-menu');

      toggle.addClass("active");
      var height = menu.children().length * 39;
      menu.css("height", height + "px");
    });

    $('.purchases-container').on('click', '.purchase-options.active', function () {
      var toggle = $(this);
      var menu = toggle.find('.purchase-options-menu');
      menu.removeAttr("style");
      toggle.removeClass("active");
    });

    $('.send-feedback-btn').click(function () {
      let that = $(this).parents('.purchase');
      let btn = $(this);

      let data = {
        orderId: btn.attr("data-order-id"),
        quantity: that.find('.item-quantity').find('span').html().match(/\d/g).join(""),
        itemTitle: that.find('.item-title').find('a').html()
      }
      openFeedbackWindow(data);
    });

    $('.purchase').on('click', '.purchase-details-btn', function () {
      let orderId = $(this).attr("data-order-id");
      let btn = $(this);
      btn.data("html", btn.html());
      btn.html("Детайли поръчка ");
      btn.append($("<i class='fa fa-circle-o-notch fa-spin fa-fw margin-bottom'>"));

      $.ajax({
        url: "/orders/details?i=" + orderId + "&r=buyer",
        method: "get",
        success: function (data) {
          if(data === "error") {
            btn.html(btn.data("html"));
            occuredError(orderId);
          } else {
              btn.html(btn.data("html"));
              showOrderDetails(data);
          }
        },
        error: function () {
          btn.html(btn.data("html"));
          occuredError(orderId);
        }
      });
    });
  })();

  (function attachDatetimeEvents() {
    $('#start-date-picker').datetimepicker({
      format: "DD.MM.YYYY"
    });

    $('#end-date-picker').datetimepicker({
      format: "DD.MM.YYYY"
      // useCurrent: false //Important! See issue #1075
    });
    $("#start-date-picker").on("dp.change", function (e) {
        $('#end-date-picker').data("DateTimePicker").minDate(e.date);
    });
    $("#end-date-picker").on("dp.change", function (e) {
        $('#start-date-picker').data("DateTimePicker").maxDate(e.date);
    });
    $('#startdate-input').on('focusout', function () {
      var interval = getIntervals();

      $('.purchase').each(function (index, el) {
        var purchaseDate = new Date($(el).attr("data-postedon"));
        purchaseDate.setHours(0);
        purchaseDate.setHours(24);

        let purchaseTime = purchaseDate;
        if(!(purchaseTime >= interval.start && purchaseTime <= interval.end))
          $(el).addClass("hidden-purchase");
        else
          $(el).removeClass("hidden-purchase");
      });
    });
    $('#enddate-input').on('focusout', function () {
      var interval = getIntervals();

      $('.purchase').each(function (index, el) {
        let purchaseTime = purchaseDate($(el).attr("data-postedon"));

        if(!(purchaseTime >= interval.start && purchaseTime <= interval.end))
          $(el).addClass("hidden-purchase");
        else
          $(el).removeClass("hidden-purchase");
      });
    });
  })();

  function getIntervals() {
    let time1 = $("#start-date-picker").data('date').split('.');
    let day = parseInt(time1[0]) + 1,
        month = time1[1],
        year = time1[2];

    let dateString = month.toString() + " " + day.toString() + " " + year.toString();
    var date = new Date(dateString);
    let startTime = date.getTime();

    let time2 = $("#end-date-picker").data('date').split('.');
    day = parseInt(time2[0]) + 1,
    month = time2[1],
    year = time2[2];

    dateString = month.toString() + " " + day.toString() + " " + year.toString();
    date = new Date(dateString);
    let endTime = date.getTime();

    return {
      start: startTime,
      end: endTime
    };
  }

  function purchaseDate(dateString) {
    var purchaseDate = new Date(dateString);
    purchaseDate.setHours(0);
    purchaseDate.setHours(24);
    let purchaseTime = purchaseDate;
    return purchaseDate;
  }

  function cancelOrderConfirmation(orderId) {
    let bodyOverlay = $('<div class="body-overlay cancel-confirmation">');
    let windowContainer = $('<div class="popup-container">');
    let title = $('<div class="popup-title">');
    let popupBody = $('<div class="popup-body">');
    let label = $('<div class="reason-title">');
    let reasonInput = $('<textarea class="reason-input" placeholder="Отказвам поръчката, понеже..">');
    let buttonsContainer = $('<div class="buttons-container">');
    let submit = $('<div class="submit-form">');
    let cancel = $('<div class="cancel-confirmation">');

    submit.click(function () {
      cancelOrder(orderId, reasonInput.val());
    });

    cancel.click(function () {
      closeOrderCancelConfirmationForm();
    });

    title.html("Отказване на поръчка");
    label.html("Причина: ");
    submit.html("ИЗПРАТИ");
    cancel.html("ОТКАЗ");

    windowContainer.css("transform", "translate(-250%, -50%)");

    buttonsContainer.append(cancel).append(submit);
    popupBody.append(label).append(reasonInput);
    windowContainer.append(title).append(popupBody).append(buttonsContainer);
    bodyOverlay.append(windowContainer);
    $('body').append(bodyOverlay);
    reasonInput.focus();

    setTimeout(function () {
      windowContainer.removeAttr("style");
    }, 300);
  }

  function closeOrderCancelConfirmationForm() {
    $('.body-overlay .popup-container').css("transform", "translate(-250%, -50%)");
    setTimeout(function () {
      $('.body-overlay').remove();
    }, 450);
  }

  function occuredError(orderId, message) {
    closeOrderCancelConfirmationForm();

    let order = $(".purchase[data-id='" + orderId + "']");
    let errorDiv = $('<div class="order-error" data-id="' + orderId + '" style="height: 0px">');
    order.find('.overlay').remove();
    order.find('.order-error').remove();

    if(message === undefined)
      errorDiv.html("Възникна грешка.Моля опитайте отново.");
    else
      errorDiv.html(message);

    order.prepend(errorDiv);
    setTimeout(function () {
      errorDiv.css("height", "35px");
    }, 200);
  }

  function cancelOrder(orderId, reason) {
    let purchase = $(".purchase[data-id='" + orderId + "']");
    let overlay = $('<div class="overlay" data-for-id="' + orderId + '">');
    let icon = $('<i class="fa fa-cog fa-spin fa-fw margin-bottom"></i>');

    overlay.append(icon);
    purchase.append(overlay);

    closeOrderCancelConfirmationForm();

    $.ajax({
      url: "/orders/cancelorder?o=" + orderId + "&r=" + escape(reason),
      method: "post",
      success: function (data) {
        if(data == "success") {
          setTimeout(function () {
            markPurchaseAsCanceled(orderId);
          }, 1000);
        } else {
            occuredError(orderId);
        }
      },
      error: function () {
        occuredError(orderId);
      }
    });
  }

  function markPurchaseAsCanceled(purchaseId) {
    let purchase = $(".purchase[data-id='" + purchaseId + "']");
    purchase.find('.overlay').remove();

    let statusField = purchase.find('.purchase-order-status').find('span');
    let icon = $('<i class="fa fa-times-rectangle">');
    statusField.html("Отказана ").append(icon);

    purchase.find('.option.cancel-order-btn').remove();
  }

  function openFeedbackWindow(model) {
    let overlay = $('<div class="feedback-overlay">');

    let popupContainer = $("<div class='feedback-window'>");
    let windowTitle = $("<div class='window-title'>");
    let closeBtn = $("<div class='close-window'>");
    let closeIcon = $('<i class="fa fa-close">');
    let starsSection = $('<div class="buyer-feedback-stars">');
    let ssTitle = $("<span class='stars-section-title'>");
    let stars = $("<div class='stars-input'>");

    for(let j = 1; j < 6; j++) {
      let i = $("<i class='fa fa-star-o'>");
      let div = $("<div class='star'>").attr("data-id", j);

      div.hover(function () {
          markStars(j);
        }, function () {
            stars.each(function (i, el) {
              $(this).find('i.fa').removeClass("fa-star").addClass("fa-star-o");
            });
            if($('.star.selected').length)
              markStars($('.star.selected').attr("data-id"));
      });
      div.click(function () {
        $('.star.selected').removeClass("selected");
        div.addClass("selected");
        markStars(j);
      });

      div.append(i).appendTo(stars);
    }

    let feedbackTitle = $("<div class='feedback-title'>");
    let feedbackInput = $('<textarea class="feedback-input">');
    let submitField = $('<div class="buttons-container">');
    let submitBtn = $("<div class='submit'>");

    submitBtn.click(function () {
      let data = {
        orderId: model.orderId,
        stars: stars.find('.star.selected').attr("data-id"),
        feedback: feedbackInput.val()
      };
      sendFeedback(data);
    });

    closeIcon.click(function () {
      hideFeedbackForm();
    });

    overlay.css("opacity", "0");
    popupContainer.css({
      "height": "0px",
      "padding": "0px",
      "box-shadow": "none"
    });

    windowTitle.html("Получих <span>" + model.quantity + "</span> броя <span>" + model.itemTitle + "</span>");
    ssTitle.html("Оценка: ");
    feedbackTitle.html("Отзив ( мнение ) :");
    submitBtn.html("ИЗПРАТИ");

    starsSection.append(ssTitle).append(stars);
    submitField.append(submitBtn);
    closeBtn.append(closeIcon);
    popupContainer.append(windowTitle)
                  .append(closeBtn)
                  .append(starsSection)
                  .append(feedbackTitle)
                  .append(feedbackInput)
                  .append(submitField)
    overlay.append(popupContainer).appendTo($("body"));

    setTimeout(function () {
      overlay.removeAttr("style");
    }, 300);
    setTimeout(function () {
      popupContainer.removeAttr("style");
    }, 700);
    feedbackInput.focus();
  }

  function markStars(count) {
    let stars = $(".stars-input").find('.star');

    stars.each(function (i, el) {
      $(this).find('i.fa').removeClass("fa-star").addClass("fa-star-o");
    });

    for(let i = 0; i < count ; i++) {
      $(stars[i]).find('i.fa-star-o').removeClass("fa-star-o").addClass('fa-star');
    }
  }

  function sendFeedback(model) {
    if(model.stars < 1 || model.stars > 5 || model.stars === undefined) {
      $('.feedback-overlay').find('.window-title').append($("<span class='val-error'>").html("Изберете оценка"));
    } else {
        $('.feedback-window').find('.buttons-container').find('.submit').html($("<i class='fa fa-circle-o-notch fa-spin fa-fw margin-bottom'>"));
        $.ajax({
          url: "/orders/sendfeedback?o=" + model.orderId + "&s=" + model.stars + "&f=" + escape(model.feedback),
          method: "post",
          success: function (data) {
            if(data === "success") {
              hideFeedbackForm();
              changePurchaseView(model.orderId);
            }
            else {
              hideFeedbackForm();
              occuredError(model.orderId);
            }
          },
          error: function (err) {
            hideFeedbackForm();
            occuredError(model.orderId);
          }
        });
    }
  }

  function hideFeedbackForm() {
    $(".feedback-window").css({
      "height": "0px",
      "padding": "0px",
      "box-shadow": "none"
    });
    setTimeout(function () {
      $(".feedback-overlay").css("opacity", "0");
    }, 800);
    setTimeout(function () {
      $('.feedback-overlay').remove();
    }, 1100);
  }

  function changePurchaseView(orderId) {
    let purchase = $('.purchase[data-id="' + orderId + '"]');
    let menu = purchase.find('.purchase-options-menu');

    let detailsBtn = $('<li class="option purchase-details-btn" data-order-id="' + orderId + '">');
    let icon = $('<i class="fa fa-list-alt">');
    detailsBtn.html("Детайли поръчка ");
    detailsBtn.append(icon);

    let statusField = purchase.find('.purchase-order-status').find('span');
    statusField.html("Завършена ");
    statusField.append($('<i class="fa fa-check-circle">'));

    menu.find('.option.send-feedback-btn').replaceWith(detailsBtn);
  }
});
