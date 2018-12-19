$(document).ready(function () {
  $('.item-thumbnail').css("height", $('.item-thumbnail').css("width"));

  $('.edit-field-btn').click(function (ev) {
    ev.stopPropagation();
    var _this = $(this);
    var input = _this.parent().parent().prev();
    _this.parent().parent().addClass("invisible");
    input.removeClass("invisible");
    setTimeout(function () {
      input.find('.field-editor-input').focus();
    }, 150);
  });

  $('.field-editor-input').focusout(function () {
    var _this = $(this);
    var valueSpan = _this.parent().next().find('.value-span');

    if(_this.val().length > 1 && _this.val() != valueSpan.html()) {
      valueSpan.html(_this.val());
    }

    _this.parent().addClass("invisible");
    _this.parent().next().removeClass("invisible");
  });

  $('#OrderBindingModel_Details').on('keydown', function(e){
    if(e.which == 13) {e.preventDefault();}
  }).on('input', function(){
      $(this).height(1);
      var totalHeight = $(this).prop('scrollHeight') - parseInt($(this).css('padding-top')) - parseInt($(this).css('padding-bottom'));
      $(this).css('height', totalHeight + 14 + "px");
  }).on('focusout', function () {
      var _this = $(this);
      var valueSpan = _this.parent().next().find('.value-span');

      if(_this.val().length > 1 && _this.val() != valueSpan.html()) {
        valueSpan.html(_this.val());
      }

      _this.parent().addClass("invisible");
      _this.parent().next().removeClass("invisible");
  });
});
