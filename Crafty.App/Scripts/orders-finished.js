$(document).ready(function () {
  (function attachPageEventHandlers() {
    // $('.hoverable').find('a').hover(function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').css("opacity", 1)
    //     a.parent().find('.popup').css("opacity", 1);
    //   },
    //   function () {
    //     var a = $(this);
    //     a.parent().find('.triangle').removeAttr('style');
    //     a.parent().find('.popup').removeAttr('style');
    // });

    $('body').on('click', function (ev) {
      // ev.stopPropagation();
      var target = $(ev.target);
      // console.log(target);
      if((!target.hasClass("option") || !target.hasClass("order-options")) && $('.order-options.active').length) {
        $('.order-options.active').find(".order-options-menu").removeAttr("style");
        $('.order-options.active').removeClass("active");
      }

      if($('.overlay').length && !$('.details-container').is(target) && $('.details-container').has(target).length === 0) {
        hideOrderDetails();
      }
    });
  })();

  (function attachOrderEventHandlers() {
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

    $('.order-details').click(function () {
      let orderId = $(this).attr("data-order-id");
      let btn = $(this);
      btn.html($("<i class='fa fa-circle-o-notch fa-spin fa-fw margin-bottom'>"))

      $.ajax({
        url: "/orders/details?i=" + orderId + "&r=seller",
        method: "get",
        success: function (data) {
          if(data === "error") {
            btn.html("детайли");
            occuredError(orderId);
          } else {
            btn.html("детайли");
            showOrderDetails(data);
          }
        },
        error: function () {
          btn.html("детайли");
          occuredError(orderId);
        }
      });

      let order = $(".order[data-id='" + orderId + "']");
    });
  })();

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
    $('#send-seller-feedback').click(function () {
      addSellerFeedbackSection();
    })

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

  function addSellerFeedbackSection() {
    let sellerFeedback = $(".seller-feedback");
    sellerFeedback.css("height", "0px");

    let sendfbBtn = $("#send-seller-feedback");
    sendfbBtn.css({
      "height": "0px",
      "border": "none",
      "padding": "0px"
    });


    let starsSection = $('<div class="seller-feedback-stars-input">');
    let starsLabel = $('<span clas="stars-label">');
    let starsInput = $('<div class="stars-input">');

    for(let j = 1; j < 6; j++) {
      let i = $("<i class='fa fa-star-o'>");
      let div = $("<div class='star'>").attr("data-id", j);

      div.hover(function () {
          markStars(j);
        }, function () {
            starsInput.each(function (i, el) {
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

      div.append(i).appendTo(starsInput);
    }

    let feedbackTitle = $("<div class='feedback-title'>");
    let feedbackInput = $('<textarea class="feedback-input">');
    let closeBtn = $("<div class='close-seller-feedback-btn'>");
    let closeIcon = $('<i class="fa fa-close">');
    let submit = $("<div class='send-feedback'>");

    closeIcon.click(function () {
      hideFeedbackForm();
    });

    starsLabel.html("Вашата оценка: ");
    feedbackTitle.html("Отзив ( мнение ) :");
    submit.html("Изпрати");

    starsSection.append(starsLabel).append(starsInput);
    closeBtn.append(closeIcon);

    submit.click(function () {
      let data = {
        orderId: $(".details-container").attr("data-order-id"),
        feedback: feedbackInput.val(),
        stars: $(".stars-input").find('i.fa.fa-star').length
      }

      let validModel = validateFeedback(data);
      if(validModel.isValid === true) {
        sendSellerFeedback(data);
      } else {
          showFeedbackValidationErrors(validModel.errors);
      }
    });

    sellerFeedback.prepend(submit)
                  .prepend(feedbackInput)
                  .prepend(feedbackTitle)
                  .prepend(starsSection);

    setTimeout(function () {
      sellerFeedback.removeAttr("style");
    }, 800);
  }

  function disableSubmitButton() {
    let btn = $('.overlay').find('.details-container').find('.send-feedback');
    btn.unbind('click');
    btn.html($("<i class='fa fa-circle-o-notch fa-spin fa-fw margin-bottom'>"));
  }

  function validateFeedback(data) {
    let validationModel = {
      isValid: true,
      errors: []
    }

    if(data.feedback.length > 200) {
      validationModel.isValid = false;
      validationModel.errors.push("Максимална дължина на мнение: 200 символа.");
    }
    if(data.stars < 1 || data.stars > 5) {
        validationModel.isValid = false;
        validationModel.errors.push("Изберете оценка от 1 до 5.");
    }

    return validationModel;
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

  function markStars(count) {
    let stars = $(".stars-input").find('.star');

    stars.each(function (i, el) {
      $(this).find('i.fa').removeClass("fa-star").addClass("fa-star-o");
    });

    for(let i = 0; i < count ; i++) {
      $(stars[i]).find('i.fa-star-o').removeClass("fa-star-o").addClass('fa-star');
    }
  }

  function sendSellerFeedback(model) {
    disableSubmitButton();
    $.ajax({
      url: "/orders/sendfeedback?o=" + model.orderId + "&s=" + model.stars + "&f=" + escape(model.feedback) + "&req=seller",
      method: "post",
      success: function (data) {
        if(data === "success")
          updateFeedbackSection(model);
        else
          occuredError(model.orderId);
      },
      error: function (err) {
        occuredError(data.orderId);
      }
    });
  }

  function updateFeedbackSection(data) {
    $(".feedback-validation-summary").css({
      "height": "0px",
      "border-top": "0px"
    });
    setTimeout(function () {
      $(".feedback-validation-summary").remove();
    }, 400);

    let stars = $("<div class='stars'>");
    let starsSpan = $("<span class='title'>");
    let starsContainer = $("<div class='stars-container'>");

    for(let i = 0; i < data.stars; i++)
    {
      let star = $("<i class='fa fa-star'>");
      starsContainer.append(star);
    }

    let feedback = $("<div class='feedback'>");
    let sectionTitle = $("<div class='section-title'>");
    let feedbackContent = $("<div class='feedback-content'>");

    starsSpan.html("Вашата оценка: ");
    sectionTitle.html("Вашето мнение: ");

    feedbackContent.html(data.feedback.replace(/(?:\r\n|\r|\n)/g, '<br />'));

    stars.append(starsSpan).append(starsContainer);
    feedback.append(sectionTitle).append(feedbackContent);

    let sellerFb = $(".seller-feedback");

    sellerFb.css("height", "0px");

    setTimeout(function () {
      sellerFb.html("");
      setTimeout(function () {
        sellerFb.append(stars).append(feedback);
      }, 100);
    }, 300);

    setTimeout(function () {
      // sellerFb.removeAttr("style");
      sellerFb.css("height", "auto");
    }, 700);
  }

  function showFeedbackValidationErrors(arr) {
    $(".feedback-validation-summary").css({
      "height": "0px",
      "border-top": "0px"
    });
    setTimeout(function () {
      $(".feedback-validation-summary").remove();

      let validationDiv = $("<div class='feedback-validation-summary'>");
      validationDiv.css({
        "height": "0px",
        "border-top": "0px"
      });

      arr.forEach(function(error){
        let span = $("<span class='error'>").append($('<i class="fa fa-caret-right">')).append("  " + error);
        validationDiv.append(span);
      });
      validationDiv.insertAfter($('.buyer-feedback'));
      setTimeout(function () {
        validationDiv.css({
          "height": arr.length * 22 + 5 + "px",
          "border-top": "5px solid #D9230F"
        });
      }, 500);
    }, 400);
  }
});
