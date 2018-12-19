var commentsSet = 1;
$(document).ready(function () {
  var changer, riAnimator;
  configSignalR();
  resizeRelatedItems();

  (function attachBodyEventListeners() {
    $(window).resize(function () {
      resizeRelatedItems();
    });

    $('.item-pictures').hover(function () {
      let box = $(this);

      if(box.find('.item-picture').length > 1)
        rotatePictures(box);
    }, function () {
        clearInterval(changer);
    });

    $('.more-comments-btn').click(function () {
      loadMoreComments(commentsSet);
    });

    $('.comments-container').on('click', '.option-edit', function () {
      let comment = $(this).closest('.comment');
      prepareCommentForEdit(comment);
    });

    $('.comments-container').on('click', '.option-delete', function () {
      let comment = $(this).closest('.comment');
      showCommentDeleteConfirmation(comment);
    });

    $('.comments-container').on('click', '.expand-options', function (ev) {
      expandOptionsMenu($(this));
    });

    $('#add-comment-input').on('keydown', function (ev) {
      if(ev.keyCode === 13)
      {
        if(ev.shiftKey)
        {
          ev.stopPropagation();
        }
        else
        {
          ev.preventDefault();
          addComment($(this).val());
        }
      }
    })

    $('.blog-title .expand-button .button-wrapper').click(function () {
      expandBlogOptionsMenu();
    });

    $('.blog-options-menu').on('click', '.option', function (ev) {
      ev.preventDefault();
      let btn = $(ev.target).parent();
      let uri = $(ev.target).attr("href");
      let type = btn.attr('class').split(' ')[1].split('-')[1];

      switch(type)
      {
        case 'archive':
          archiveBlog(uri, btn.attr("data-id"));
          break;

        case 'delete':
          deleteBlog(uri);
          break;

        case 'edit':
          window.location.href = "/blogs/edit/" + btn.attr("data-id");
          break;

        default: break;
      }
    });

    $(document).on('click', function (ev) {
      ev.stopPropagation();
      let target = $(ev.target);
      if($('.login-dynamic').length && $('.login-dynamic').has(target).length === 0 && !$('.login-dynamic').is(target)) {
        hideDynamicLogin();
      }

      // if(checkElement('.edit-comment-input', target))
      //   returnComment();

      if(checkElement('.comment-options.expanded', target))
        closeCommentMenu();

      if(checkElement('.blog-options', target))
        expandBlogOptionsMenu('close');
    });

    autoResize([$('#add-comment-input')]);
    startRIAnimations();
    startCommentDatesRefresher();
  })();

  function resizeRelatedItems() {
    $.each($('.related-items-container .related-item'), function () {
      $(this).css('height', $(this).css('width'));
    });
  }

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

  function closeCommentMenu() {
    $('.comment-options.expanded').removeClass("expanded");
  }

  function expandOptionsMenu(btn) {
    let menu = btn.parent();
    if(menu.hasClass("expanded"))
    {
      menu.removeClass("expanded");
    }
    else
    {
      $('.comment-options.expanded').removeClass("expanded");
      menu.addClass("expanded");
    }
  }

  function rotatePictures(container) {
    let box = container;
    let pics = box.find('.item-picture');
    let picsCount = pics.length;
    let it = 0;
    changer = setInterval(function (){
      let current = $(pics[it]);
      let next = $(pics[it + 1]);
      current.css('opacity', '0');
      if(next.length)
        next.css('opacity', '1');
      else
        pics.first().css('opacity', '1');

      it++;
      if(it == picsCount)
        it = 0;
    }, 1000);
  }

  function startRIAnimations() {
    let itemsCount = $('.related-items .related-item').length - 1;

    setInterval(function () {
      let index = randomInt(0, itemsCount);
      let element = $('.related-item')[index];
      $(element).addClass('animated');
      setTimeout(function () {
        $('.related-item.animated').removeClass('animated');
      }, 2000);
    }, 8000);
  }

  var datesRefresher;
  function startCommentDatesRefresher() {
    datesRefresher = setInterval(function () {
      let ids = [];
      $.each($('.comments-container .comment'), function(i, comment) {
        let _comment = $(comment);
        let commentId = _comment.attr('data-id');
        ids.push(commentId);
      });
      getFreshDate(ids.join(','));
    }, 60000);
  }

  function loadMoreComments(page) {
    let blogId = $('.blog-wrapper').attr('data-id');
    appendLoadingRow();
    $.ajax({
      method: "get",
      url: "/comments/blogcomments/" + blogId + "?p=" + page,
      success: function (data) {

        if(data !== "error")
          appendComments(data);
      },
      error: function (err) {
        showMoreCommentsError();
      }
    }).done(function () {
        removeLoadingRow();
    });
  }

  function appendLoadingRow() {
    let div = $('<div class="loading-row">');
    let icon = $('<i class="fa fa-spinner fa-pulse fa-2x fa-fw margin-bottom">');
    icon.appendTo(div);
    div.insertAfter($('.comments-container .comment').last());
  }

  function removeLoadingRow() {
    $('.comments-container .loading-row').remove();
  }

  function appendComments(model) {
    let count = 0;
    let htmlString = model;
    let re = new RegExp('class="comment"', 'gi')
    while(re.exec(htmlString))
    {
      count++;
    }

    if(count < 15)
      $('.more-comments-btn').remove();
    else
      commentsSet++;

    $('.comments-container').append(model);
  }

  function getFreshDate(ids) {
    $.ajax({
      url: "/comments/getdate?arr=" + ids,
      method: "get",
      success: function(response) {
        if(response.data != "error")
        {
          $.each(response.data, function(i, element) {
            $('.comment[data-id="' + element.Id + '"]').find('.comment-posted-on').html(element.Date);
          });
        }
      },
      error: function (err) {
      }
    });
  }

  function autoResize(inputs) {
    for(let i = 0; i < inputs.length; i++)
    {
      let input = $(inputs[i]);
      input.on('keydown', function(ev){
        if(ev.which == 13 ) {
          if(ev.shiftKey)
            ev.stopPropagation();
        }
      }).on('input', function(){
          $(this).height(1);
          var totalHeight = $(this).prop('scrollHeight') - parseInt($(this).css('padding-top')) - parseInt($(this).css('padding-bottom'));
          $(this).css('height', totalHeight + 14 + "px");
      });
    }
  }

  function addComment(text) {
    let blogId = $('.blog-wrapper').attr('data-id');
    $.ajax({
      method: 'post',
      url: '/comments/addblogcomment?b=' + blogId + '&c=' + text,
      success: function(data) {
        if(data !== "success")
          showDynamicLogin(data);
        else
          $('#add-comment-input').val("");
      },
      error: function(err) {
        console.log(err.responseText);
      }
    });
  }

  function randomInt(min,max) {
    return Math.floor(Math.random()*(max-min+1)+min);
  }

  function configSignalR() {
    var hub = $.connection.comments;

    hub.client.updateBlogComments = function(model) {
      prependComment(model);
    }

    hub.client.updateCommentEdit = function (content, id) {
      refreshComment(id, content);
    }

    $.connection.hub.start().fail(function (err) {
      console.log(err);
    });
  }

  function prependComment(comment) {
    let commentContainer = $("<div class='comment new' data-id='" + comment.Id + "'>");
    let apContainer = $("<div class='author-picture'>");
    let profiPic = $('<img src="' + comment.UserPicture + '">');
    let commentInfo = $('<div class="comment-info">');
    let authorName = $('<div class="comment-author">').html(comment.UserName);
    let postedOn = $('<div class="comment-posted-on">').html(comment.PostedOn);
    let commentContent = $('<div class="comment-content">').html(comment.Content.replace(/(?:\r\n|\r|\n)/g, '<br />'));

    apContainer.append(profiPic);
    commentInfo.append([authorName, postedOn, commentContent]);
    commentContainer.append([apContainer, commentInfo]);

    if($('.comments-container').length)
    {
      commentContainer.insertBefore($('.comments-container .comment').first());
    }
    else
    {
      ($('<div class="comments-container">').append(commentContainer)).insertAfter($('.add-comment-section'));
    }

    incrementCommentsCount(1);
  }

  function incrementCommentsCount(num) {
    if($('.comments-title .comments-count-value').length)
    {
      $('.comments-count-value').html(parseInt($('.comments-count-value').html()) + 1);
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

  function prepareCommentForEdit(comment) {
    let text = comment.find('.comment-content').html();
    let textarea = $('<textarea class="edit-comment-input" cols="1" rows="1">');
    textarea.attr('last-content', text);

    autoResize(textarea);
    textarea.val(text);
    comment.find('.comment-content').replaceWith(textarea);

    textarea.keydown(function (ev) {
      if(ev.keyCode === 13)
      {
        if(ev.shiftKey)
        {
          ev.stopPropagation();
        }
        else
        {
          ev.preventDefault();
          submitCommentEdit(textarea.val(), textarea.closest('.comment').attr("data-id"));
        }
      }
      else if(ev.keyCode === 27)
      {
        returnComment();
      }
    });

    textarea.focus().ready(function () {
      setTimeout(function () {
        $(document).on('click', removeCommentEdit);
      }, 1000);
    });

    closeCommentMenu();
  }

  function removeCommentEdit(ev) {
    let target = $(ev.target);

    if(checkElement('.edit-comment-input', target))
      returnComment();
  }

  function submitCommentEdit(text, id) {
    hideCommentError(id);
    $.ajax({
      url: "/comments/editBlogComment/" + id + "?content=" + text,
      method: "post",
      success: function(data) {
        if(data === "error")
          showCommentError(id);
      },
      error: function (err) {
        showCommentError(id);
      }
    });
  }

  function showCommentError(id) {
    let comment = $('.comment[data-id="' + id + '"]');
    comment.addClass("error");
  }

  function returnComment(newContent) {
    $(document).unbind('click', removeCommentEdit);
    let input = $('.comments-container textarea.edit-comment-input');
    let div = $('<div class="comment-content">')

    let oldContent = input.attr('last-content');
    div.html(oldContent);

    $(input).replaceWith(div);
  }

  function refreshComment(id, newContent) {
    returnComment(newContent);
    let comment = $('.comment[data-id="' + id + '"]');
    comment.find('.comment-content').html(newContent);
    comment.addClass("updated");
    setTimeout(function () {
      comment.removeClass("updated");
    }, 60000);
  }

  function showCommentDeleteConfirmation(comment) {
    closeCommentMenu();

    let popup = $('<div class="comment-delete-confirmation-overlay">');
    let contentWrapper = $('<div class="content-wrapper">');
    let container = $('<div class="message-container">');
    let title = $('<div class="message-title">').html("Сигурни ли сте, че искате да изтриете този коментар?");
    let buttonsContainer = $('<div class="buttons-container">');
    let acceptBtn = $('<div class="accept-button">').html("Изтрий");
    let denyBtn = $('<div class="deny-button">').html("Отказ");

    acceptBtn.click(function () {
      deleteComment(comment.attr("data-id"));
    });

    denyBtn.click(function () {
      hideCommentDeleteConfirmation();
    });

    buttonsContainer.append([denyBtn, acceptBtn]);
    container.append([title, buttonsContainer]).appendTo(contentWrapper);
    popup.append(contentWrapper).appendTo($('body'));

    setTimeout(function () {
      $(document).on('click', commentConfirmationWatcher);
    }, 100);
  }

  function commentConfirmationWatcher(ev) {
    let target = $(ev.target);

    if(checkElement('.comment-delete-confirmation-overlay .message-container', target))
      hideCommentDeleteConfirmation();
  }

  function hideCommentDeleteConfirmation() {
    $('.comment-delete-confirmation-overlay').remove();

    $(document).unbind('click', commentConfirmationWatcher);
  }

  function expandBlogOptionsMenu(action) {
    let menu = $('.blog-options');
    if(action === undefined)
    {
      if(menu.hasClass("expanded"))
        menu.removeClass("expanded");
      else
        menu.addClass("expanded");
    }
    else
    {
      if(action === "close")
        menu.removeClass("expanded");
    }
  }

  function archiveBlog(uri, id) {
    $.ajax({
      method: "get",
      url: uri,
      success: function (data) {
        if(data === "success")
        {
          if(uri.indexOf("blogs/archive") > -1)
            changeArchiveBtn("show", id);
          else
            changeArchiveBtn("archive", id);
        }
        else
          showBlogError("archive");
      },
      error: function () {
        showBlogError("archive");
      }
    });
  }

  function deleteBlog(uri) {
    $.ajax({
      method: "post",
      url: uri,
      success: function (data) {
        if(data === "success")
          window.location.href = "/";
        else
          showBlogError("delete");
      },
      error: function() {
        showBlogError("delete");
      }
    });
  }

  function changeArchiveBtn(type, id) {
    let btn = $('.blog-options-menu .option[data-id="' + id + '"]');
    switch(type)
    {
      case "show":
        var newContent = $('<a href="/blogs/show/' + id + '">').append("Възобнови ").append($('<i class="fa fa-refresh">'));
        btn.html("").append(newContent);
        break;

      case "archive":
        var newContent = $('<a href="/blogs/archive/' + id + '">').append("Архивирай ").append($('<i class="fa fa-archive">'));
        btn.html("").append(newContent);
        break;

      default: break;
    }
  }

  function deleteComment(id) {
    hideCommentError(id);
    $.ajax({
      url: '/comments/deleteBlogComment/' + id,
      method: "post",
      success: function (data) {
        if(data === "success")
        {
          $('.comments-container .comment[data-id="' + id + '"]').remove();
          hideCommentDeleteConfirmation();
        }
        else
        {
          showCommentError(id);
        }
      },
      error: function (err) {
        console.log(err);
      }
    });
  }

  function hideCommentError(id) {
    let comment = $('.comment[data-id="' + id + '"]');
    comment.removeClass("error");
  }
});
