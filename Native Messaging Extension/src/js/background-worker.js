var port = null;
var openedTabId = null;

function openUrl(url){
  if(!openedTabId){
    openNewWindow(url);
  }
  else{
    chrome.tabs.update(openedTabId, { url: url});
  }
}

async function openNewWindow(url){
  let window = await chrome.windows.create({
    focused : true,
    height: 450,
    width:800,
    focused: true,
    type: "popup",
    url: url
  });
  openedTabId = window.tabs[0].id;
  chrome.windows.onRemoved.addListener(onWindowClosed);
}

function onNativeMessage(message) {
  if(message.Operation && message.Value){
    if(message.Operation == "Open"){
      openUrl(message.Value);
    }
  }
} 

function onWindowClosed(id) {
  openedTabId = null;
}

function onDisconnected() {
  port = null;
}

var hostName = "com.my_company.my_application";
port = chrome.runtime.connectNative(hostName);
port.onMessage.addListener(onNativeMessage);
port.onDisconnect.addListener(onDisconnected);