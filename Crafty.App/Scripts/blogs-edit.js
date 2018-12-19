$(document).ready(function () {
  $.fn.extend({
    autoResize: function(startSet) {
      if(startSet === true)
      {
        $(this).height(1);
        var totalHeight = $(this).prop('scrollHeight') - parseInt($(this).css('padding-top')) - parseInt($(this).css('padding-bottom'));
        $(this).css('height', totalHeight + 14 + "px");
        focusedElement = $(this);
      }

      this.focusout(function () {
        if($(this).val().length === 0)
        {
          $(this).remove();
          focusedElement = undefined;
        }
      }).on('keydown', function(e){
        if(e.which == 13) {
          $(this).css('height', parseInt($(this).css('height') + 27));
        }
      }).on('input', function(){
          $(this).height(1);
          var totalHeight = $(this).prop('scrollHeight') - parseInt($(this).css('padding-top')) - parseInt($(this).css('padding-bottom'));
          $(this).css('height', totalHeight + 14 + "px");
          focusedElement = $(this);
      })
    }
  });
  var focusedElement = $('.blog-content-input').first();
  var lastInsertedImage = 0;
  var lastInsertedParagraph = 0;
  var blogContentId;

  convertBlogContent();
  (function attachBodyEventListeners() {
    $('.blog-content-input').on('input', function(){
        $(this).height(1);
        var totalHeight = $(this).prop('scrollHeight') - parseInt($(this).css('padding-top')) - parseInt($(this).css('padding-bottom'));
        $(this).css('height', totalHeight + 14 + "px");
        focusedElement = $(this);
    });
    // .on('focusout', function () {
    //     var _this = $(this);
    //     var valueSpan = _this.parent().next().find('.value-span');
    //
    //     if(_this.val().length > 1 && _this.val() != valueSpan.html()) {
    //       valueSpan.html(_this.val());
    //     }
    //
    //     _this.parent().addClass("invisible");
    //     _this.parent().next().removeClass("invisible");
    // });

    $('body').on('wheel', function(event) {
      //
    });

    $('.form-wrapper').on('click', '.remove-related-item', function (ev) {
      let btn = $(ev.target);
      btn.closest('.related-item.active').remove();
    })

    $(document).on('click', function (ev) {
      let target = $(ev.target);

      if($('.related-item-input-wrapper').length && $('.related-item-input-wrapper').has(target).length === 0 && !$('.related-item-input-wrapper').is(target) && !$('.related-item.add-related-item').is(target))
        hideRelatedItemInput();
    });

    $('.content-section').on('focus', '.blog-content-input', function (ev) {
      focusedElement = $(ev.target);
    });

    $('.tool.add-image').click(function () {
      appendImageUpload();
    });

    $('.tool.add-paragraph').click(function () {
      appendParagraph();
    });

    $('.tool.bold-text').click(function (ev) {
      if(focusedElement != undefined) {
        focusedElement.selection('insert', { text: '<b>', mode: 'before' });
        focusedElement.selection('insert', { text: '</b>', mode: 'after' });
        focusedElement.focus();
        focusedElement = undefined;
      }
    });

    $('.tool.italic-text').click(function (ev) {
      if(focusedElement != undefined) {
        focusedElement.selection('insert', { text: '<i>', mode: 'before' });
        focusedElement.selection('insert', { text: '</i>', mode: 'after' });
        focusedElement.focus();
        focusedElement = undefined;
      }
    });

    $('.tool.link').click(function () {
      if(focusedElement != undefined)
      {
        focusedElement.selection('insert', { text: ' <a class="content-link" target="_blank" href="ЛИНК">ТЕКСТ</а> ', mode: 'after' });
        focusedElement.focus();
        focusedElement = undefined;
      }
    });

    $('.tool.youtube-video').click(function () {
      appendYTVideoInput();
    });

    $('.related-item.add-related-item').click(function () {
      showRelatedItemInput();
    });

    $('.buttons-section #submit').click(function () {
      submitForm();
    });
  })();

  function convertBlogContent() {
    let container = $('.content-section #current-content');
    let content = container.children();
    let imgId = parseInt(container.attr('data-last-img-id')) - 1;
    let textboxId = 1;
    blogContentId = container.attr('data-blog-content-id');
    // container.remove();

    let inputs = [];

    let outputHTML = $('<div class="content-section">');
    $.each(content, function () {
      let element = $(this);
      let _class = element.attr('class').split(' ')[0];

      switch(_class)
      {
        case 'content-paragraph':
          textboxId++;
          let textbox = $('<textarea class="blog-content-input" data-id="' + textboxId + '" cols="0" rows="0">').val(element.html().replace(/\&nbsp;/g, ' ').replace(/<br\s*\/?>/mg,"\n\r"));
          outputHTML.append(textbox);
          inputs.push(textbox);
          break;

        case 'content-image':
          imgId++;
          let imgDiv = $('<div class="content-image-port" data-id="' + imgId + '">');
          let img = $('<img class="content-image" data-id="' + imgId + '"src="' + element.find('img').attr('src') + '">');
          let removeBtn = $('<div class="remove-image-btn" data-id="' + imgId + '">');
          let icon = $('<i class="fa fa-minus-circle">').appendTo(removeBtn);

          removeBtn.click(function () {
            deleteContentImage(removeBtn.attr('data-id'));
          });

          if(img.height < 450)
            imgDiv.css('height', img.height());
          else
            imgDiv.css('height', '450px');

          imgDiv.append([img, removeBtn]).appendTo(outputHTML);
          break;

        case 'content-video':
          let videoWrapper = $('<div class="video-content-input">');
          let videoLink = element.html();
          let input = $('<input class="link-input" placeholder="Постави кода тук" type="text">').val(videoLink);
          let videoBox = $('<div class="video-wrapper">').html(videoLink);
          videoWrapper.append([input, videoBox]).appendTo(outputHTML);
          break;

        default: break;
      }
    });
    $('.content-section').replaceWith(outputHTML);

    $.each(inputs, function () {
      $(this).autoResize(true);
    });

    lastInsertedImage = imgId;
    lastInsertedParagraph = textboxId;
  }

  function appendImageUpload() {
    lastInsertedImage += 1;
    let portWrapper = $('<div class="content-image-port" data-id="' + lastInsertedImage + '" style="background: rgba(0, 0, 0, 0);border: none">');
    let uploadPort = $('<input type="file" id="NewImages" name="NewImages" class="content-image-input" accept="image/*" data-id="' + lastInsertedImage + '">');

    uploadPort.change(function () {
      let id = parseInt($(this).attr("data-id"));
      readURL(this, id);
    });

    portWrapper.append(uploadPort);

    if(focusedElement != undefined)
      portWrapper.insertAfter(focusedElement);
    else
      $('.content-section').append(portWrapper);

    // $('.content-section').append(portWrapper);
    uploadPort.trigger('click');
    setTimeout(function () {
      if(uploadPort.get(0).files.length === 0)
        portWrapper.remove();
    }, 5000);
  }

  function appendParagraph() {
    lastInsertedParagraph += 1;
    let textInput = $('<textarea class="blog-content-input" data-id="' + lastInsertedParagraph + '" cols="10" rows="10">');
    let container = $('.content-section');

    textInput.autoResize();

    if(focusedElement != undefined)
      textInput.insertAfter(focusedElement)
    else
      container.append(textInput);

    textInput.focus();
  }

  function appendYTVideoInput() {
    let portWrapper = $('<div class="video-content-input">');
    let linkInput = $('<input class="link-input" type="text" placeholder="Постави кода тук">');

    linkInput.on('keyup', debounce(function () {
      loadYTVideoPreview(linkInput.val(), portWrapper);
    }, 1000));

    linkInput.focusout(function () {
      if($(this).val().length === 0)
        portWrapper.remove();
    })

    portWrapper.append(linkInput)

    if(focusedElement != undefined)
      $('.content-section').append(portWrapper);
    else
      portWrapper.insertAfter(focusedElement);

    linkInput.focus();
  }

  function readURL(input, inputId) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = function (e) {
        showUploadedImage(inputId, e.target.result);
      }

      reader.readAsDataURL(input.files[0]);
    }
  }

  function showUploadedImage(id, src) {
    let input = $('.content-image-input[data-id="' + id + '"]');
    let image = $('<img class="content-image" data-id="' + id + '" src="' + src + '">');
    let container = $('.content-image-port[data-id="' + id + '"]');

    container.append(image);
    container.removeAttr("style");

    if(image.height() < 450)
      container.css('height', image.height());
    else
      container.css('height', '450px');

    let removeImage = $('<div class="remove-image-btn" data-id="' + id + '">');
    let icon = $("<i class='fa fa-minus-circle'>").appendTo(removeImage);
    removeImage.click(function () {
      deleteContentImage(id);
    });
    container.append(removeImage);
  }

  function deleteContentImage(id) {
    let container = $('.content-image-port[data-id="' + id + '"]');
    let filePort = container.find('.content-image-input');

    if(filePort !== undefined)
      filePort.val("");

    container.remove();
  }

  function submitForm() {
    let modelState = validateForm();

    if(!modelState.valid)
    {
      showValidationErrors(modelState.errors);
    }
    else
    {
      let parsedContent = parseBlogContent();
      $('.form-wrapper #Content').val(parsedContent.fetchedHtml);
      $('.form-wrapper #BlogContentIdentifier').val(parsedContent.contentIdentifier);
      $('.form-wrapper #UpdatedRelatedItems').val(getRelatedItems());
      $('.form-wrapper form').submit();
    }
  }

  function validateForm() {
    clearValidationErrors();
    let modelState = {
      valid: true,
      errors: {}
    }

    let textInputs = $('.content-section .blog-content-input');
    let textLengthTotal = 0;
    $.each(textInputs, function () {
      let input = $(this);
      textLengthTotal += input.val().length;
    });
    if(textLengthTotal < 30)
    {
      modelState.valid = false;
      modelState.errors.contentValidation = "Съдържанието на статията трябва да е поне 30 символа";
    }

    if($('.form-wrapper .title-input').val().length < 5)
    {
      modelState.valid = false;
      modelState.errors.titleValidation = "Заглавието трябва да е дълго поне 5 символа.";
    }

    return modelState;
  }

  function getRelatedItems() {
    let ids = [];
    $.each($('.related-item.active'), function () {
      ids.push($(this).attr('data-item-id'));
    });
    return ids.join(",");
  }

  function showValidationErrors(list) {
    for(var err in list)
    {
      if(list.hasOwnProperty(err))
      {
        switch(err)
        {
          case "contentValidation":
            showContentValidationError(list[err]);
            break;
          case "titleValidation":
            showTitleValidationError(list[err]);
            break;
          default: break;
        }
      }
    }
  }

  function showContentValidationError(message) {
    let errDiv = $('<div class="validation-error content-validation-error" style="min-height: 0px;height: 0px;padding: 0px">');
    errDiv.html(message);
    errDiv.insertBefore($('.content-section'));
    setTimeout(function () {
      errDiv.removeAttr("style");
    }, 500);
  }

  function showTitleValidationError(message) {
    let errDiv = $('<div class="validation-error title-validation-error" style="min-height: 0px;height: 0px;padding: 0px">');
    errDiv.html(message);
    errDiv.insertBefore($('.title-section'));
    setTimeout(function () {
      errDiv.removeAttr("style");
    }, 500);
  }

  function clearValidationErrors() {
    let errors = $('.validation-error');
    $.each(errors, function () {
      $(this).css({
        'height': '0px',
        'min-height': '0px',
        'padding': '0px'
      });
      setTimeout(function () {
        $(this).remove();
      }, 400);
    });
  }

  function parseBlogContent() {
    let content = $('.content-section');
    let result = {
      fetchedHtml: "",
      contentIdentifier: blogContentId != undefined ? blogContentId : iD()
    };
    $.each(content.children(), function() {
      let element = $(this);
      let _class = element.attr('class').split(' ')[0];
      switch(_class)
      {
        case "blog-content-input":
          if(element.hasClass('blog-content-input'))
          {
            let paragraph = '<div class="content-paragraph">' + element.val().replace(/(?:\r\n|\r|\n)/g, '<br />') + '</div>';
            result.fetchedHtml += paragraph;
          }
          break;

        case "content-image-port":
          if(element.find('.content-image-port').length)
          {
            let imgId = element.attr('data-id');
            let imgExtension = element.find('.content-image-input').val().split('.');
            imgExtension = '.' + imgExtension[imgExtension.length - 1];
            let imageDiv = '<div class="content-image" data-id="' + imgId + '"><img data-id="' + imgId + '" src="/images/blogs/' + result.contentIdentifier +  '-' + imgId + imgExtension + '"/></div>';
            result.fetchedHtml += imageDiv;
          }
          else
          {
            let imgId = element.attr('data-id');
            let imgSrc = element.find('.content-image').attr('src');
            let imageDiv = '<div class="content-image" data-id="' + imgId + '"><img data-id="' + imgId + '" src="' + imgSrc + '"></div>';
            result.fetchedHtml += imageDiv;
          }
          break;

        case "video-content-input":
          let frameWrapper = element.find('.video-wrapper').html();
          let videoDiv = '<div class="content-video">' + frameWrapper + '</div>';
          result.fetchedHtml += videoDiv;
          break;
        default: break;
      }
    });
    return result;
  }

  function debounce(func, wait, immediate) {
  	var timeout;
  	return function() {
  		var context = this, args = arguments;
  		var later = function() {
  			timeout = null;
  			if (!immediate) func.apply(context, args);
  		};
  		var callNow = immediate && !timeout;
  		clearTimeout(timeout);
  		timeout = setTimeout(later, wait);
  		if (callNow) func.apply(context, args);
  	};
  };

  function loadYTVideoPreview(code, vidWrapper) {
    let _code = code.replace('</iframe>', '');
    let frameWrapper = $('<div class="video-wrapper">');
    let frame = $(_code).addClass('youtube-video-frame');
    vidWrapper.find('.video-wrapper').remove();
    frameWrapper.append(frame);
    vidWrapper.append(frameWrapper);
  }

  function showRelatedItemInput() {
    let existingInput = $('.related-item-input');
    if(existingInput.length)
      return existingInput.focus();

    let riInputWrapper = $('<div class="related-item-input-wrapper">');
    let suggestionsWrapper = $('<div class="related-items-suggestions">');
    let riInput = $('<input class="related-item-input" type="text" style="height: 0px;padding: 0px;border: 0px" placeholder="Име на изделие">');

    riInput.on('keyup', debounce(function () {
      if(riInput.val().length > 0)
        showRelatedItemsSuggestions(riInput.val());
    }, 1000));

    riInputWrapper.append(suggestionsWrapper).append(riInput);
    riInputWrapper.appendTo($('body'));

    setTimeout(function () {
      riInput.removeAttr("style");
    }, 230);

    riInput.focus();
  }

  function showRelatedItemsSuggestions(target) {
    $.ajax({
      method: "get",
      url: "/search/relatedItems?title=" + target,
      success: function(data) {
        loadRelItemsSuggestions(data);
      },
      error: function(err) {
        console.log(err.responseText);
      }
    });
  }

  function loadRelItemsSuggestions(model) {
    let suggestionsContainer = $('.related-items-suggestions');

    suggestionsContainer.html("");

    model.forEach(function (element) {
      let suggestDiv = $('<div class="suggestion-item">').attr('data-id', element.Id);
      let thumbnail = $('<div class="suggestion-thumbnail">').append($("<img>").attr('src', element.Thumbnail));
      let suggestionInfo = $('<div class="suggestion-info">');
      let suggestionTitle = $('<div class="suggestion-title">').html(element.Title);
      let suggestionPrice = $('<div class="suggestion-price">').html(element.Price + "лв.");

      suggestDiv.click(function () {
        chooseRelItemSuggestion(element);
      });

      suggestionInfo.append([suggestionTitle, suggestionPrice]);
      suggestDiv.append([thumbnail, suggestionInfo]);
      suggestDiv.appendTo(suggestionsContainer);
    });

    if(model.length * 70 > 420)
      suggestionsContainer.perfectScrollbar();
  }

  let relItemId = 1;
  function chooseRelItemSuggestion(model) {
    let existingItem = $('.related-item.active[data-item-id="' + model.Id + '"]');
    if(existingItem.length)
      return highlightExistingItem(model.Id);

    let relativeItem = $('<div class="related-item active">').attr({
      'data-item-id': model.Id
    });
    let relItemWrapper = $('<div class="related-item-wrapper">');
    let relItemThumnail = $('<div class="related-item-thumbnail">').append($('<img>').attr('src', model.Thumbnail));
    let relItemInfo = $('<div class="related-item-info">');
    let relItemTitle = $('<div class="related-item-title">').html(model.Title);
    let relItemPrice = $('<div class="related-item-price">').html(model.Price + "лв.");
    let removeItemBtn = $('<div class="remove-related-item">').html("ИЗТРИЙ");

    relItemInfo.append([relItemTitle, relItemPrice]);
    relItemWrapper.append([relItemThumnail, relItemInfo, removeItemBtn]);
    relativeItem.append(relItemWrapper);
    relativeItem.insertAfter($('.related-items-section .related-items-section-title'));
  }

  function hideRelatedItemInput() {
    let inputWrapper = $('.related-item-input-wrapper');

    inputWrapper.css({
      'padding': '0px',
      'max-height': '0px',
      'margin': '0px',
      'border': 'none'
    });

    setTimeout(function () {
      inputWrapper.remove();
    }, 400);
  }

  function highlightExistingItem(id) {
    let item = $('.related-item[data-item-id="' + id + '"]');

    item.addClass("highlited");
    setTimeout(function () {
      item.removeClass("highlited");
    }, 400);
  }

  var iD = function () {
    return '_' + Math.random().toString(36).substr(2, 9);
  }
});
