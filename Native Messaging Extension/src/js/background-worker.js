var port = null;
var openedTabId = null;
var openedWindowId = null;

function openUrl(url) {
    if (!openedTabId) {
        openNewWindow(url);
    }
    else {
        navigateToUrl(openedTabId, url);
    }
}

function isEmpty(str) {
    return (!str || str.length === 0 );
}

function isUrl(str) {
    return (str && str.match(/((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)/) );
}

function isNumber(str) {
    return (str || str.match(/\d+/) );
}

function navigateToUrl(tabId, newUrl) {
    if( !tabId || !isUrl(newUrl) ){
        return;
    }
    chrome.tabs.update(tabId, { url: newUrl });
}

function resizeWindow(windowId, newWidth, newHeight) {
    if(!windowId || !isNumber(newHeight) || !isNumber(newWidth) ){
        return;
    }
    chrome.windows.update(windowId, { height: parseInt(newHeight), width: parseInt(newWidth) });
}

async function calcCenterWindow(width, height){
    const displayInfos = await chrome.system.display.getInfo();
    let point = {
        left : 0,
        top : 0
    }
    if( displayInfos.length > 0 ){
        const displayBounds = displayInfos[0].bounds;
        point.left = ~~((displayBounds.width - width) / 2);
        point.top = ~~((displayBounds.height - height) / 2);
    }
    return point;
}

async function centerWindow(windowId, width, height){
    const point = await calcCenterWindow(width, height);
    chrome.windows.update(windowId, { left: point.left, top: point.top });
}

async function openNewWindow(url) {
    if(!isUrl(url) ){
        return;
    }
    const width = 800;
    const height = 450;
    const point = await calcCenterWindow(width, height);
    let window = await chrome.windows.create({
        focused: true,
        height: height,
        width: width,
        left: point.left, 
        top: point.top,
        focused: true,
        type: "popup",
        url: url
    });
    openedWindowId = window.id;
    openedTabId = window.tabs[0].id;
    chrome.windows.onRemoved.addListener(onWindowClosed);
}

function onNativeMessage(message) {
    if (message.Operation && message.Value) {
        const value = message.Value;
        const operation = message.Operation.toLowerCase();
        if (operation == "open") {
            openUrl(value);
        }
        else if (operation == "resize" && value.match(/\d+x\d+/) ) {
            const splitArray = value.split('x');
            resizeWindow(openedWindowId, splitArray[0], splitArray[1]);
        }
    }
}

function onWindowClosed() {
    openedTabId = null;
}

function onDisconnected() {
    port = null;
}

var hostName = "com.my_company.my_application";
port = chrome.runtime.connectNative(hostName);
port.onMessage.addListener(onNativeMessage);
port.onDisconnect.addListener(onDisconnected);