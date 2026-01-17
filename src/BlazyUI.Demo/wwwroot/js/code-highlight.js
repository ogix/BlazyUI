window.codeHighlight = {
    highlight: function (elementId) {
        const element = document.getElementById(elementId);
        if (element && !element.dataset.highlighted) {
            hljs.highlightElement(element);
        }
    },
    copyToClipboard: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.error('Failed to copy:', err);
            return false;
        }
    }
};
