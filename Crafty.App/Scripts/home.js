$(document).ready(function () {
  let currentLayer = 1;
  (function attachBodyEventListeners() {
    startFirstLayer();

    if($('.blogs-wrapper .blog-section').length > 1)
    {
      startBlogsContainer();
      configureNavigators();
    }

    $('body').on('wheel', function(event) {
      let windowHeight = $(window).innerHeight();
      let scrollHeight = $(window).scrollTop();
      // if (event.originalEvent.deltaY > 0) {
      $('.landing-layer-text').css({
        'bottom': 'calc(7% + ' + scrollHeight/2 + 'px)'
      });
      // }
      // else
      // {
      //   $('.landing-layer-text').css({
      //     'bottom': 'calc(7% +)'
      //   });
      // }

      // show circles
      let windowWidth = $(window).innerWidth();
      if(windowWidth < 712)
      {
        if(scrollHeight > windowHeight - 200)
        {
          let parts = $('.second-layer .part');
          $(parts[0]).css('margin-top', '0px');
          setTimeout(function () {
            $(parts[1]).css('margin-top', '0px');

            setTimeout(function () {
              $(parts[2]).css('margin-top', '0px');
            }, 180);
          }, 180);
        }
      }
    });
  }());

  function startFirstLayer() {
    setInterval(function () {
      let layer = $('.first-layer').find('.part.part-' + currentLayer);
      layer.css('opacity', '0');

      if(currentLayer == 3)
      {
        layer.prev().prev().css('opacity', '1');
      }
      else
      {
        layer.next().css('opacity', '1');
      }

      if(currentLayer == 3)
        currentLayer = 1;
      else
        currentLayer++;
    }, 6000);
  }

  var blogsTimer;
  function startBlogsContainer()
  {
    blogsTimer = setInterval(function () {
      changeSlide();
    }, 5000);
  }

  function configureNavigators() {
    $('.navigators .carousel i.fa').hover(function (ev) {
      let carousel = $(ev.target);
      if(carousel.hasClass('fa-circle-o'))
        carousel.addClass('fa-circle');
    }, function (ev) {
        let carousel = $(ev.target);
        if(carousel.hasClass('fa-circle-o') && carousel.hasClass('fa-circle'))
          carousel.removeClass('fa-circle');
    });

    $('.navigators .carousel').click(function (ev) {
      let target = $(this);
      changeSlide('current', target.attr('data-slide-id'));
    });
  }

  function changeSlide(direction, id) {
    let width = $('.blogs-wrapper').css('width');
    switch(direction)
    {
      case 'next':
        let active = $('.navigators .fa.fa-circle');
        let carousel = active.parent();
        if(carousel.next().length)
        {
          active.removeClass('fa-circle').addClass('fa-circle-o');
          carousel.next().find('i.fa').removeClass('fa-circle-o').addClass('fa-circle');

          let margin =  parseFloat("-" + carousel.next().attr('data-slide-id') * 100) + 100;
          $('.blogs-wrapper').css({
            'margin-left': margin + 'vw',
            'width': width
          });
        }
        else
        {
          active.removeClass('fa-circle').addClass('fa-circle-o');
          $('.navigators .carousel').first().find('i.fa').removeClass('fa-circle-o').addClass('fa-circle');
          $('.blogs-wrapper').css({
            'width': width,
            'margin-left': "0vw"
          });
        }
        break;

      case 'current':
        clearInterval(blogsTimer);

        let nextCarousel = $('.navigators .carousel[data-slide-id="' + id + '"]');
        let margin = parseFloat('-' + id * 100) + 100;
        $('.navigators .fa.fa-circle').each(function () {
          $(this).removeClass('fa-circle').addClass('fa-circle-o');
        });
        nextCarousel.find('i.fa').removeClass('fa-circle-o').addClass('fa-circle');
        $('.blogs-wrapper').css({
          'margin-left': margin + 'vw',
          'width': width
        });

        startBlogsContainer();
        break;
      default:
        changeSlide('next');
        break;
    }
  }
});
