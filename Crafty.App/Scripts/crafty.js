$(document).ready(function (){

  $('body').on('wheel', function(event) {
    if(window.innerWidth > 767)
    {
      if (event.originalEvent.deltaY > 0) {
        $('.navbar.navbar-inverse.navbar-main').css({
          'top': '-50px',
          'margin-bottom': '0px'
        });
        $('.account-nav').css({
          'top': '0px'
        });
        $('.categories-navigation-left').css({'top': '0px'});
        $('.container.body-content').css({
          'margin-top': '0px'
        });
      } else {
          $('.navbar.navbar-inverse.navbar-main').css({
            'top': '0px',
            'margin-bottom': '18px'
          });
          $('.account-nav').css({
            'top': '50px'
          });
          $('.categories-navigation-left').css({'top': '50px'});
          $('.container.body-content').css({
            'margin-top': '50px'
          });
      }
    }
  });

  sessionStorage.notifSkipNum = 7;
  sessionStorage.disableScrollNotifLoad = false;

  var connectionId = null;
  (function configSignalR() {
    var hub = $.connection.notifications;

    hub.client.receiveNotification = function (id) {
      incrementNotifCount(1);

      addNotification(id);
    }

    $.connection.hub.start().done(function () {
      connectionId = $.connection.hub.id;
    });

    function addNotification(id) {
      $.ajax({
        url: "/notifications/get/" + id,
        method: "get",
        success: function (data) {
          if(data != "error") {
            if($('.notifications-container').length > 0) {
              $('.notifications-wrapper').prepend(data);
            } else {
                popNotification(data);
            }
          } else {
              alert("Възникна грешка");
          }

        },
        error: function (err) {
          alert(err.responseText);
        }
      });
    }
  })();

  (function pageChecks() {
    if($('.columns').length) {
      $('.columns').on('click', '.item-author', function (ev) {
        ev.preventDefault();
        ev.stopPropagation();
        window.location.href = "/account/users/" + $(this).html();
      });
    }

    if(window.location.href.toLowerCase().indexOf("account") > -1) {
      $(".menu-wrapper").on('click', '.toggle-scallable', function (ev) {
        ev.preventDefault();
        ev.stopPropagation();

        var target = $(this);
        if(!target.hasClass("toggle-sclbl-active")) {
          target.addClass("toggle-sclbl-active");
          target.next().css("height", target.next().children().length * 31 + "px");
        } else {
          target.removeClass("toggle-sclbl-active");
          target.next().removeAttr("style");
        }
      });

      // CHANGE THIS AFTER DEPLOYMENT TO DOMAIN NAME
      var params = window.location.href.toString().replace("http://localhost:11264", "").split("?");
      params = params[0].split("/");
      for(var i in params) {
        if(params[i].length == 0 || params[i] === "")
        params.splice(params.indexOf(params[i]), 1);
      }

      if(params.length == 1 || params[1].toLowerCase() === "index")
        $("#index-tab").addClass("active");
      else if(params[1].length > 10)
        return;
      else
        $("#" + params[1].toLowerCase() + "-tab").addClass("active");
    }

    if(window.location.href.toLowerCase().indexOf("orders") > -1) {
      let btn = $('#manager-tab').find('.toggle-scallable');
      btn.addClass("toggle-sclbl-active");
      btn.next().css("height", btn.next().children().length * 31 + "px");

      $(".menu-wrapper").on('click', '.toggle-scallable', function (ev) {
        ev.preventDefault();
        ev.stopPropagation();

        var target = $(this);
        if(!target.hasClass("toggle-sclbl-active")) {
          target.addClass("toggle-sclbl-active");
          target.next().css("height", target.next().children().length * 31 + "px");
        } else {
          target.removeClass("toggle-sclbl-active");
          target.next().removeAttr("style");
        }
      });
      var params = window.location.href.toString().replace("http://localhost:11264", "").split("?");
      params = params[0].split("/");
      for(var i in params) {
        if(params[i].length == 0 || params[i] === "")
        params.splice(params.indexOf(params[i]), 1);
      }

      $("#option-" + params[1].toLowerCase()).addClass("active");
    }
  })();

  (function attachBodyEventListeners() {
    $(".dropdown-toggle").dropdown();

    $('.navbar-toggle').click(function () {
      hideAccountNavigation();
    });

    $('.hide-category').click(function (ev) {
      ev.preventDefault();

      hideCategory($(ev.target).attr("data-categ-id"));
    });

    $('.show-category').click(function (ev) {
      ev.preventDefault();

      showCategory($(ev.target).attr('data-categ-id'));
    });

    $('#notifications-btn').on('click', '.notifications-title', function (ev) { ev.stopPropagation(); });

    $('.searchbox-wrap').find('input[name="search_word"]').on('keyup', function (ev) {
      let input = $(this);
      if (ev.keyCode == 13 && input.val().length > 0) {
        window.location.href = "/search/word/" + $('input[name="search_word"]').val();
      } else if(ev.keyCode === 27) {
          closeSearchBox();
      }
    });

    $(document).on('click', function (ev) {
      var target = $(ev.target);

      if(($('.notifications-container').find('.notification').length || $('.notifications-container').find('.error-message').length) && checkElement('.notifications-container', target)) {
        hideNotificationsContainer();
      }
    });

    $('#notifications-btn').on('click', '.notification a.inner-wrapper', function (ev) {
      ev.preventDefault();
      let btn = $(this);
      let url = btn.attr('href');

      let notif = btn.closest('.notification');
      let notifId = notif.attr("data-id");

      if(notif.hasClass("unseen"))
        markNotificationAsRead(notifId, true, url);
      else
        window.location.href = url;
    });

    $("#notifications-btn").on('click', '.mark-read-btn .icon', function (ev) {
      ev.stopPropagation();
      let btn = $(ev.target);
      if(btn.hasClass('unseen')) {
        let id = btn.parent().attr('data-id');
        markNotificationAsRead(id, false);
      }
    });

    $(".icn-search").click(function () {
      var searchbox = $(".searchbox-wrap");

      if (!$(".searchbox-wrap").hasClass("active")) {
        openSearchBox();
      } else {
          if($('input[name="search_word"]').val().length > 0) {
            window.location.href = "/search/word/" + $('input[name="search_word"]').val();
          } else {
              closeSearchBox();
          }
      }
    });

    $("#toggle-account-menu").click(function (ev) {
      var _this = $(this);
      var nav_menu = $(".account-nav");
      if(nav_menu.hasClass("toggle_active")) {
        return hideAccountNavigation();
      }
      _this.html("СКРИЙ");
      _this.css({
        "left": "270px",
        "z-index": "9"
      });
      return nav_menu.addClass("toggle_active");
    });

    $('#close-reg-info').click(function () {
      $('.register-tab').find(".reg-info-container").remove();
      $('.register-tab').find('.triangle').remove();
      $.ajax({
        url: '/account/HideRegInfo',
        method: "get",
        success: function(){},
        error: function () {}
      });
    });

    $('#notifications-btn').click(function (ev) {
      var btn = $(this);
      if(!btn.hasClass("opened")) {
        if(window.innerWidth <= 767) {
          $('.footer').hide();
        }
        btn.addClass("opened");
        btn.css({
          "overflow": "visible"
        });

        let notificationsContainer = $('<div class="notifications-container">');
        let notifWrapper = $("<div class='notifications-wrapper'>");
        let secTitle = $("<div class='notifications-title'>")
                        .append($('<span class="subtitle">').html("Известия"));

        let smallScreenBtn = $('<i class="fa fa-close" id="close">').appendTo(secTitle);
        let markAllRead = $('<span id="mark-all-read">').html("Маркирай всички като прочетени.").prependTo(secTitle);
        let loadingIcon = $("<i class='fa fa-cog fa-spin fa-2x fa-fw margin-bottom'>");

        let notificationsBtn = $("#notifications-btn");

        markAllRead.click(function () {
          allRead();
        });

        smallScreenBtn.click(function () {
          hideNotificationsContainer();
        });

        notifWrapper.append(loadingIcon);
        notificationsContainer.append(secTitle).append(notifWrapper);
        notificationsContainer.appendTo(notificationsBtn);

        getNotifications();
      } else {
          hideNotificationsContainer();
      }
    });
  })();

  function checkElement(selector, target, timeout) {
    if(timeout === undefined)
    {
      if($(selector).length && $(selector).has(target).length === 0 && !$(selector).is(target) && !$(selector).is('focus'))
        return true;
      return false;
    }
    else
    {
      setTimeout(function () {
        if($(selector).length && $(selector).has(target).length === 0 && !$(selector).is(target))
          return true;
        return false;
      }, timeout);
    }
  }

  function hideAccountNavigation() {
    let menuBtn = $("#toggle-account-menu");
    menuBtn.html("МЕНЮ");
    menuBtn.removeAttr("style");
    $(".account-nav").removeClass("toggle_active");
  }

  function openSearchBox() {
    $(".icn-search").addClass("active");
    $(".searchbox-wrap").addClass('active').removeClass('fix');
    $(".searchbox-wrap input").addClass("active");
    $(".btm_border").addClass("active");
    $('#search-box-li').addClass("no-hover");
    $('.searchbox-wrap input').focus();
  }

  function closeSearchBox() {
    $(".icn-search").removeClass("active");
    $(".searchbox-wrap input").removeClass("active");
    $(".btm_border").removeClass("active");
    $(".searchbox-wrap").removeClass('active').addClass('fix');
    setTimeout(function () {
      $('#search-box-li').removeClass("no-hover");
    }, 1300);
  }

  function hideCategory(id) {
    $.ajax({
      url: "/items/hidecateg/" + id,
      method: "post",
      success:function (data) {
        if(data === "success")
          refreshCategoryRow(id, "show");
        else
          alert(data);
      },
      error: function (err) {
        alert(err.responseText);
      }
    });
  }

  function showCategory(id) {
    $.ajax({
      url: "/items/showcateg/" + id,
      method: "post",
      success:function (data) {
        if(data === "success")
          refreshCategoryRow(id, "hide");
        else
          alert(data);
      },
      error: function (err) {
        alert(err.responseText);
      }
    });
  }

  function refreshCategoryRow(id, type) {
    $('.nav.navbar-nav .dropdown').addClass('open');
    $('.nav.navbar-nav .dropdown-toggle').attr('aria-expanded', 'true');

    let category = $('.category[data-id="' + id + '"]');
    let newIcon;

    console.log(type);

    if(type == "show")
    {
      newIcon = $('<i class="fa fa-plus-circle show-category" data-categ-id="' + id + '">');
      newIcon.click(function (ev) {
        ev.preventDefault();

       showCategory(id);
      });
    }
    else if(type == "hide")
    {
      newIcon = $('<i class="fa fa-minus-circle hide-category" data-categ-id="' + id + '">');
      newIcon.click(function (ev) {
        ev.preventDefault();

        hideCategory(id);
      });
    }

    category.find('i.fa').replaceWith(newIcon);
  }

  function markNotificationAsRead(notifId, redirect, url) {
    $.ajax({
      url: "/notifications/markread?i=" + notifId,
      method: "post",
      success: function () {
        if(redirect === true) {
          window.location.href = url;
        } else {
            refreshNotificationsMenu(notifId);
        }
      },
      error: function () {
        if(redirect === true) {
          window.location.href = url;
        }
      }
    });
  }

  function refreshNotificationsMenu(notifId) {
    let notification = $('.notification[data-id="' + notifId + '"]');
    notification.removeClass("unseen");
    notification.find('.icon.unseen').removeClass('unseen');

    decrementNotifCount(1);
  }

  function decrementNotifCount(n) {
    let notifCount = $('.notifications-count');
    let currentVal = parseInt(notifCount.html());

    if(notifCount.length && currentVal > 2) {
      let newVal = currentVal - n;
      notifCount.html(newVal);
    } else {
        notifCount.remove();
    }
  }

  function incrementNotifCount(n) {
    let notifCount = $('.notifications-count');
    let currentVal = parseInt(notifCount.html());

    if(notifCount.length && currentVal > 0) {
      let newVal = parseInt(notifCount.html()) + n;
      notifCount.html(newVal);
    } else {
        $('#notifications-btn').prepend($("<div class='notifications-count'>").html("1"));
    }
  }

  function getNotifications() {
    $.ajax({
      url: "/notifications/all",
      method: "get",
      success: function (data) {
        if(data === "error") {
          $('.notifications-container').find('i.fa').replaceWith("<span class='error-message'>Възникна грешка, моля опитай отново.</span>");
        } else {
            if(data.toString().length > 1) {
              $('.notifications-wrapper').html("").append(data);
              $('.notifications-wrapper').perfectScrollbar();
              attachMoreNotificationsScroll();
            } else {
                $('.notifications-wrapper').find("i.fa").replaceWith("<span class='error-message'>Възникна грешка, моля опитай отново.</span>");
            }

        }
      },
      error: function (err) {
        return err.responseText;
      }
    });
  }

  function attachMoreNotificationsScroll() {
    let wrapper = $('.notifications-wrapper');
    wrapper.on('ps-y-reach-end', function () {
      if(sessionStorage.disableScrollNotifLoad == "false") {
        getMoreNotifications(sessionStorage.notifSkipNum);
        sessionStorage.notifSkipNum =  parseInt(sessionStorage.notifSkipNum) + 7;
      }
    });
  }

  function getMoreNotifications(skipNum) {
    $.ajax({
      url: "/notifications/all/" + skipNum,
      method: "get",
      success: function (data) {
        if(data != "error") {
          if(data.length > 1) {
            appendNextNotfications(data);
          } else {
              sessionStorage.disableScrollNotifLoad = true;
          }
        } else {
            notificationsError();
        }
      },
      error: function (err) {
        notificationsError();
      }
    })
  }

  function appendNextNotfications(html) {
    let lastNotif = $('.notifications-wrapper').find('.notification').last();

    $(html).insertAfter(lastNotif);
  }

  function notificationsError(message) {
    let msg = message != undefined ? message : "Възникна грешка.";
    let lastNotif = $('.notifications-wrapper').find('.notification').last();

    let errorNotif = $('<div class="notifications-error">').html(msg);
    errorNotif.insertAfter(lastNotif);
  }

  function hideNotificationsContainer() {
    $('.notifications-container').css({
      "height": "0px",
      "overflow": "hidden",
      "box-shadow": "none",
      "padding": "0px"
    });
    $('#notifications-btn').removeClass("opened");
    setTimeout(function () {
      $('.notifications-container').remove();
    }, 400);

    if(window.innerWidth <= 767) {
      $('.footer').show();
    }

    sessionStorage.disableScrollNotifLoad = false;
    sessionStorage.notifSkipNum = 7;
  }

  function popNotification(htmlView) {
    if(window.innerWidth <= 767) {
      if($('#notitication-line').children().length === 0) {
        let notifLine = $("#notifications-line");
        let notif = $(htmlView);
        notif.css({
          height: "0px",
          overflow: "hidden",
          padding: "0px"
        });
        notifLine.append(notif);

        setTimeout(function () {
          notifLine.css({
            "z-index": "9999",
            "height": "74px"
          });
          notif.removeAttr("style");


          setTimeout(function () {
            notif.css({
              height: "0px",
              overflow: "hidden",
              padding: "0px"
            });
            setTimeout(function () {
              notif.remove();
            }, 350);
          }, 5000);

        }, 350);
      }
    } else {
        let notifLine = $("#notifications-line");
        let notif = $(htmlView);
        notif.css({
          height: "0px",
          overflow: "hidden",
          padding: "0px"
        });
        notifLine.append(notif);

        setTimeout(function () {
          notif.removeAttr("style");

          setTimeout(function () {
            notif.css({
              height: "0px",
              overflow: "hidden",
              padding: "0px"
            });
            setTimeout(function () {
              notif.remove();
            }, 350);
          }, 5000);

        }, 350);
    }
  }

  function allRead() {
    $.ajax({
      url: "/notifications/allread?go=true",
      method: "post",
      success: function () {
        $('.notifications-count').remove();
        $('.notification.unseen').each(function () {
          markNotificationAsRead($(this).attr("data-id"), false);
        });
      },
      error: function () {
        notificationsError();
      }
    });
  }
});
