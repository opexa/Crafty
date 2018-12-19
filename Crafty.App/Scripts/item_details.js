$(document).ready(function () {
  (function extendJQuery() {
    $.fn.extend({
      disableCarousel: function () {
        let carousel = $(this);
        carousel.addClass("disabled");
      },
      enableCarousel: function () {
        let carousel = $(this);
        carousel.removeClass("disabled");
      }
    });
  })();

  (function attachBodyEventListeners() {
    if(window.innerWidth >= 1396) {
      carousels();
      $('.carousel-up').disableCarousel();
      $('.carousel-left').disableCarousel();
    } else {
        configurePicturesSlider();
        if(window.innerWidth < 601) {
          configureSuggestionsSlider(5);
        } else {
            configureSuggestionsSlider(10);
        }
    }

    $(document).click(function (ev) {
      var target = $(ev.target);
      if(!$(".item-options").has(target).length && !$(".item-options").hasClass("hidden-options")) {
        $(".item-options").addClass("hidden-options");
      }

      if($('.login-dynamic').length && $('.login-dynamic').has(target).length === 0 && !$('.login-dynamic').is(target)) {
        hideDynamicLogin();
      }
    });

    $('.item-picture').click(function (){
      var _this = $(this);
      $('.main-picture-wrapper img').attr("src",  _this.find('img').attr("src"));
    });

    $('.like-item-button').click(function () {
      var _this = $(this);
      $.ajax({
        url: "/items/like/" + _this.attr("data-item-id"),
        method: "get",
        success: function(data) {
          if(data === "liked") {
            liked();
          }
          else if(data === "unliked") {
            unlike();
          }
          else {
            showDynamicLogin(data);
          }
        },
        error: function (err) {
          console.log(err.responseText);
        }
      });
    });

    $("#item-to-favs").click(function (ev){
      ev.stopPropagation();
      let itemId = $(this).attr("data-item-id");

      $.ajax({
        method: "post",
        url: "/items/favourite/" + itemId,
        success: function(data) {
          changeButton(data);
        },
        error: function (err) {
          console.log(err.responseText);
        }
      });
    });

    $(".more-suggestions").click(function () {
      var _this = $(this);
      $.ajax({
        method: "get",
        url: "/items/suggestions/" + _this.attr("data-for-id"),
        success: function (data) {
          $(data).insertBefore(_this);
          _this.remove();
        },
        error: function (err) {
          console.log(err.responseText);
        }
      });
    });

    $("#order-submit-btn").click(function () {
      let amount = parseInt($("#order-amount-input").val());

      if(amount < 1) {
        $("#val-error").html("Минималното количество за поръчка е 1 бр.");
      } else {
        $("#loading-availability").removeClass("hidden");
        $("#val-error").html("");
        checkAvailability(amount);
      }
    });

    $('.item-options').click(function () {
      let _this = $(this);
      if(_this.hasClass("hidden-options"))
      return _this.removeClass("hidden-options");
      return _this.addClass("hidden-options");
    });

    $('#place-order').click(function () {
      let btn = $(this);
      if(btn.hasClass('active')) {
        checkAvailability($('#order-quantity').val());
      } else {
          btn.addClass("active");
          let input = $("#order-quantity");
          input.removeAttr("style");
          input.focus();
          btn.hover(function () {
            btn.css({
              "border-left": "none"
            });
            input.css({
              border: "2px solid rgb(0, 97, 210)",
              "border-right": "none"
            });
          }, function () {
            input.removeAttr("style");
            btn.removeAttr("style");
          });
      }
    })
  }());

  function configureSuggestionsSlider(margin) {
    let scWidth = parseInt($('.items-section').css('width').replace("px", ""));
    let single = parseInt($('.suggestion').css('width').replace("px", ""));
    let suggestions = $('.suggestion').length;
    let suggWidth = suggestions*single + (suggestions-2)*margin*2 + 2*margin + (suggestions-1)*4;
    if(scWidth < suggWidth) {
      $('.suggestions-container').css("width", suggWidth + "px");
      new Dragdealer('tablet-slider');
    }
  }

  function configurePicturesSlider() {
    // if(navigator.userAgent.match(/iPad/i) && navigatoor.userAgent.match(/Mobile|Windows Phone|Lumia|Android|webOS|iPhone|iPod|Blackberry|PlayBook|BB10|Opera Mini|\bCrMo\/|Opera Mobi/i)) {
    let ipWidth = parseInt($('.item-pictures').css("width").replace("px", ""));
    let pics = $('.item-picture').length;
    let width = pics*150 + (pics - 2)*10 + 2*5 + (pics - 1)*4;
    if(window.innerWidth < 1211 && ipWidth < width) {
      $('.carousel-down').hide();
      $('.carousel-up').hide();

      $('.pictures-container').css("width", width + "px");
      new Dragdealer('pictures-slider');
    } else {
        carousels('pictures');
        $('.carousel-up').disableCarousel();
    }
    // }
  }

  function carousels(which) {
    if(which != undefined && which === "pictures") {
      let carUp = $('.carousel-up');
      let carDown = $('.carousel-down');
      $('.pictures-container').addClass("transition");
      carUp.click(function () {
        if(!carUp.hasClass("disabled"))
          picturesPrev();
      });
      carDown.click(function () {
        if(!carDown.hasClass("disabled"))
          picturesNext();
      });
    } else {
        let carUp = $('.carousel-up');
        let carDown = $('.carousel-down');
        $('.pictures-container').addClass("transition");
        carUp.click(function () {
          if(!carUp.hasClass("disabled"))
            picturesPrev();
        });
        carDown.click(function () {
          if(!carDown.hasClass("disabled"))
            picturesNext();
        });

        let carLeft = $('.carousel-left');
        let carRight = $('.carousel-right');

        $('.suggestions-container').addClass("transition");
        carLeft.click(function () {
          if(!carLeft.hasClass("disabled"))
            suggestionsPrev();
        });
        carRight.click(function () {
          if(!carRight.hasClass("disabled"))
            suggestionsNext();
        });
    }
  }

  function suggestionsPrev() {
    let container = $('.suggestions-container');

    let itemsCount = container.find('.suggestion').length;

    switch(itemsCount)
    {
      case 5:
      case 6:
      case 7:
        container.css("margin-left", "0px");
        $('.carousel-left').disableCarousel();
        break;
      case 8:
      case 9:
      case 10:
        if(!container.attr("plus-click")) {
          container.css("margin-left", "-586px");
        } else {
            container.css("margin-left", "0px");
            $('.carousel-left').disableCarousel();
        }
        break;
      default: break;
    }

    let plusClick = container.attr("plus-click");
    if(plusClick != undefined && plusClick === "true") {
      container.removeAttr("plus-click");
    } else {
      container.attr("plus-click", "true");
    }

    $('.carousel-right').enableCarousel();
  }

  function suggestionsNext() {
    let container = $('.suggestions-container');
    let itemsCount = container.find('.suggestion').length;


    switch(itemsCount)
    {
      case 5:
        container.css("margin-left", "-142px");
        $('.carousel-right').disableCarousel();
        break;
      case 6:
        container.css("margin-left", "-435px");
        $('.carousel-right').disableCarousel();
        break;
      case 7:
        container.css("margin-left", "-728px");
        $('.carousel-right').disableCarousel();
        break;
      case 8:
        if(!container.attr("plus-click")) {
          container.css("margin-left", "-728px");
        } else {
            $('.carousel-right').disableCarousel();
            container.css("margin-left", "-1022px");
        }
        break;
      case 9:
        if(!container.attr("plus-click")) {
          container.css("margin-left", "-728px");
        } else {
            $('.carousel-right').disableCarousel();
            container.css("margin-left", "-1317px");
        }
        break;
      case 10:
        if(!container.attr("plus-click")) {
          container.css("margin-left", "-728px");
        } else {
            $('.carousel-right').disableCarousel();
            container.css("margin-left", "-1610px");
        }
        break;
      default: break;
    }

    let plusClick = container.attr("plus-click");
    if(plusClick != undefined && plusClick === "true") {
      container.removeAttr("plus-click");
    } else {
        container.attr("plus-click", "true");
    }

    $('.carousel-left').enableCarousel();
  }

  function picturesPrev() {
    let container = $('.pictures-container');
    let itemsCount = container.find('.item-picture').length;

    switch(itemsCount)
    {
      case 4:
      case 5:
      case 6:
        container.css("margin-top", "0px");
        $('.carousel-up').disableCarousel();
        break;
      case 7:
      case 8:
      case 9:
        if(!container.attr("plus-click")) {
          container.css("margin-top", "-480px");
        } else {
            $('.carousel-up').disableCarousel();
            container.css("margin-top", "0px");
        }
        break;
      default: break;
    }

    let plusClick = container.attr("plus-click");
    if(plusClick != undefined && plusClick === "true") {
      container.removeAttr("plus-click");
    } else {
        container.attr("plus-click", "true");
    }
    $('.carousel-down').enableCarousel();
  }

  function picturesNext() {
    let container = $('.pictures-container');
    let itemsCount = container.find('.item-picture').length;

    switch(itemsCount)
    {
      case 4:
      case 5:
      case 6:
        container.css("margin-top", "-400px");
        $('.carousel-down').disableCarousel();
        break;
      case 7:
      case 8:
      case 9:
        if(!container.attr("plus-click")) {
          container.css("margin-top", "-480px");
        } else {
            $('.carousel-down').disableCarousel();
            container.css("margin-top", "-890px");
        }
        break;
      default: break;
    }

    let plusClick = container.attr("plus-click");
    if(plusClick != undefined && plusClick === "true") {
      container.removeAttr("plus-click");
    } else {
        container.attr("plus-click", "true");
    }
    $('.carousel-up').enableCarousel();
  }

  function resizeImages() {
    // $.each($('.item-pictures img'), function (index, element) {
    //   var _this = $(this);
    //   $(this).css("height", _this.css("width"));
    // });
    $('.main-picture-wrapper').css("height", $('.main-picture-wrapper').css("width"));
  }

  function changeButton(state) {
    if(state === "added") {
      $("#item-to-favs").html("МАХНИ ОТ ЛЮБИМИ").append($("<i>").addClass("glyphicon glyphicon-star"));
    }
    else if(state === "removed") {
      return $("#item-to-favs").html("ДОБАВИ В ЛЮБИМИ").append($("<i>").addClass("glyphicon glyphicon-star-empty"));
    }
    else {
      showDynamicLogin(state);
    }
  }

  function liked() {
    var likeBtn = $('.like-item-button');
    likeBtn.html("Не харесвам");
    likeBtn.removeClass("liked-false").addClass("liked-true");
    var likesSec = $('.likes-counter');
    var likesInt = likesSec.find('.int');
    if(likesInt.length) {
      likesInt.html(parseInt(likesInt.html()) + 1);
    } else {
        likesSec.html("");
        likesSec.append($("<i>").addClass('fa fa-thumbs-o-up')).append("&nbsp;");
        likesSec.append(($("<span>").addClass("int")).html("1"));
    }
  }

  function unlike() {
    var likeBtn = $('.like-item-button');
    likeBtn.html("Харесай");
    likeBtn.removeClass("liked-true").addClass("liked-false");
    var likesSec = $('.likes-counter');
    var likesInt = likesSec.find('.int');
    if(parseInt(likesInt.html()) > 1) {
      likesInt.html(parseInt(likesInt.html()) - 1);
    } else {
        likesSec.html("Бъди първия харесал");
    }
  }

  function showDynamicLogin(html) {
    if(!$('.login-dynamic').length) {
      $('body').append(html);
      setTimeout(function (){
        $('.login-dynamic').removeAttr("style");
      }, 500);
    }
  }

  function hideDynamicLogin() {
    let form = $('.login-dynamic');
    form.css({
      height: "0px"
    });
    setTimeout(function () {
      form.remove();
    }, 300);
  }

  function checkAvailability(amount) {
    let itemId = $('.item-container').attr("data-id");
    $.ajax({
      url: "/items/availability/" + itemId + "?amount=" + amount,
      method: "get",
      success: function (data) {
        $("#loading-availability").addClass("hidden");
        if(data == "available") {
          window.location.href = "/orders/place?i=" + itemId + "&q=" + amount;
        } else if(data == "not-found") {
            $("#val-error").html("Възникна грешка. Моля презаредете страницата.");
        } else {
            $("#val-error").html("Заявената бройка надвишава наличното количество: " + data + " бр.");
            $(".availability-value").html(data + " бр.");
        }
      },
      error: function (err) {
        console.log("error: " + err.responseText);
      }
    })
  }
});
