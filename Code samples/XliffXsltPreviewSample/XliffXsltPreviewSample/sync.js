(function () {
	function post(message) {
		if (window.chrome && window.chrome.webview) {
			window.chrome.webview.postMessage(message);
		}
	}

	var style = document.createElement('style');
	style.textContent = '.sdl-seg-active { background: #fff3b0; outline: 2px solid #f0b400; }';
	(document.head || document.documentElement).appendChild(style);

	function select(id) {
		Array.prototype.forEach.call(document.querySelectorAll('.sdl-seg-active'), function (e) {
			e.classList.remove('sdl-seg-active');
		});
		var anchors = Array.prototype.filter.call(document.querySelectorAll('.sdl-seg'), function (a) {
			return a.getAttribute('data-seg') === id;
		});
		anchors.forEach(function (a) { a.parentElement.classList.add('sdl-seg-active'); });
		return anchors;
	}

	window.onSegmentSelectedInEditor = function onSegmentSelectedInEditor(id) {
		var anchors = select(id);
		if (!anchors.length) {
			post({ type: 'cannotNavigateToSegment', segmentId: id });
			return;
		}
		anchors[0].parentElement.scrollIntoView({ block: 'center' });
	};

	document.addEventListener('click', function (e) {
		var anchor = e.target.querySelector ? e.target.querySelector('.sdl-seg') : null;
		if (!anchor) {
			var all = document.querySelectorAll('.sdl-seg');
			for (var i = all.length - 1; i >= 0; i--) {
				if (all[i].compareDocumentPosition(e.target) & Node.DOCUMENT_POSITION_FOLLOWING) {
					anchor = all[i];
					break;
				}
			}
		}
		if (anchor) {
			var id = anchor.getAttribute('data-seg');
			select(id);
			post({ type: 'selectSegment', segmentId: id });
		}
	});

	post({ type: 'applyInitialSelection' });
})();
