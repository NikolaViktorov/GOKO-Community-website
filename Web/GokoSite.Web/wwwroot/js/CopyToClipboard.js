function copyTextToClipboard(text) {
    var textarea = document.createElement('textarea');
    textarea.textContent = text;
    document.body.appendChild(textarea);

    var selection = document.getSelection();
    var range = document.createRange();
    //  range.selectNodeContents(textarea);
    range.selectNode(textarea);
    selection.removeAllRanges();
    selection.addRange(range);

    console.log('copy success', document.execCommand('copy'));
    selection.removeAllRanges();

    document.body.removeChild(textarea);
}

function CopyLink() {
    let url = window.location.href;
    url = url.replace("CollectionDetails", "SharedGame");
    copyTextToClipboard(url);
}