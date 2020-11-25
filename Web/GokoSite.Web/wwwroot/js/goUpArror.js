;(function () {
	'use strict'; var goToTop = function () {

	$('.js-gotop').on('click', function (event) {

		event.preventDefault();

		var body = $("html, body");
		body.stop().animate({ scrollTop: 0 }, 500, 'swing', function () {
		});

		return false;
	});

	$(window).scroll(function () {
		var $win = $(window);
		if ($win.scrollTop() > 200) {
			$('.js-top').addClass('active');
		} else {
			$('.js-top').removeClass('active');
		}

	});

	};

	$(function () {
		goToTop();
	});

}());