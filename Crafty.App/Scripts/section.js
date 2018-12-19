$(document).ready(function () {
  var sectionId = $('.section-row.selected').attr('data-id');

  if(window.location.href.indexOf("ctg") > -1) {
    var categoryId = window.location.href.slice(window.location.href.indexOf("ctg"), window.location.href.indexOf("ctg") + 6).replace("ctg=", "");
    var category = $('.category-row[data-id="' + categoryId + '"]');
    category.addClass("selected");
    $('#current-category-name').html(category.html());
  }
  else {
    var categ = $('.sections-navigation').find('.section-row.selected').find('.categories-navigation').children().first();
    categ.addClass("selected");
    $('#current-category-name').html(categ.html());
  }

  $('.category-row').click(function () {
    var _this = $(this);
    var current = _this.parent().find('.selected');
    $('#current-category-name').html(_this.html());
    if(_this[0] != current[0]) {
      current.removeClass('selected');
      _this.addClass('selected');
      $.ajax({
        url: "/items/category/" + _this.attr("data-id") + "?skipNum=1",
        method: "get",
        success: function(data) {
          $('.columns').animate({'opacity': 0}, 400, function(){
            document.title = _this.html();
            scrlAjax = 2;
            breakpoint = one;
            window.history.pushState(_this.html(), _this.html(), "/items/section/" + sectionId + "?ctg=" + _this.attr('data-id'));
            $(this).html(data).animate({'opacity': 1}, 400);
          });
        },
        error: function(err) {
          console.log(err.responseText);
        }
      });
    }
  });

  var scrlAjax = 2;
  var one = ($(window).height() * 2) / 3;
  var breakpoint = one;
  var categId = window.location.href.slice(window.location.href.indexOf("ctg="), window.location.href.indexOf("ctg=") + 6).replace("ctg=", "");
  $(window).scroll(function () {
    if($(window).scrollTop() > breakpoint) {
      breakpoint += one;
      return $.ajax({
        url: "/items/category/" + categId + "?skipNum=" + scrlAjax,
        method: "get",
        success: function(data) {
          $('.columns').append(data);
          scrlAjax += 1;
        },
        error: function (err) {
          console.log(err.responseText);
        }
      });
    }
  });

  $("#toggle-sections-menu").click(function () {
    var _this = $(this);
    var nav_menu = $(".categories-navigation-left");
    if(nav_menu.hasClass("toggle_active")) {
      _this.html("MENЮ");
      _this.removeAttr("style");
      return nav_menu.removeClass("toggle_active");
    }
    _this.html("СКРИЙ");
    _this.css({
      "left": "220px",
      "z-index": "9"
    });
    return nav_menu.addClass("toggle_active");
  });
});
