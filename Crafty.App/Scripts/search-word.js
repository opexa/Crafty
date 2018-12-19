$(document).ready(function () {
  (function attachBodyEventListeners() {
    $('.search-word').click(function () {  openDynamicSearch(); });

    $(document).click(function (ev) {
      let target = $(ev.target);

      if($('#new-search').length && !$(target).is($("#new-search")) && !$(target).is("#new-search-btn")) {
        refreshSearchInputSection(27);
      }
    })
  }());

  function openDynamicSearch() {
    if($('#new-search').length === 0) {
      let searchWord = $('.search-word');

      let input = $('<input id="new-search" type="text">').val(searchWord.html());

      input.keyup(function (ev) {
        refreshSearchInputSection(ev.keyCode);
      });

      searchWord.css({
        opacity: "0",
        width: "0",
        height: "25px",
        overflow: "hidden",
        display: "inline-block"
      });

      setTimeout(function () {
        searchWord.css("display", "none");
        input.insertAfter(searchWord);
        input.focus();
      }, 200);
    }
  }

  function refreshSearchInputSection(keyCode) {
    let input = $("#new-search");
    let goIcon = $('<i id="new-search-btn" class="fa fa-play-circle">');

    if (keyCode == 13) {
        window.location.href = "/search/word/" + input.val();
    } else if (keyCode == 27) {
        input.css({
          width: "0px",
          opacity: "0px"
        });
        setTimeout(function () {
          input.remove();
          $('#new-search-btn').remove();
          $('.search-word').removeAttr("style");
        }, 180);
    } else if(input.val().length > 0) {
        $('.page-heading #new-search-btn').remove();
        goIcon.insertAfter(input);
        goIcon.click(function () {
          window.location.href = "/search/word/" + input.val();
        });
    } else {
        $("#new-search-btn").remove();
    }
  }
});
