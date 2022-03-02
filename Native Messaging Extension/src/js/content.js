/*******************************************************************************
    Native Messaging Erweiterung
    Copyright (C) Christian Szech
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see {http://www.gnu.org/licenses/}.
*/

'use strict';

var port = chrome.runtime.connectNative('com.my_company.my_application');

port.onMessage.addListener(function(msg) {
    console.log("Received" + msg);
});

port.onDisconnect.addListener(function() {
    console.log("Disconnected");
});

port.postMessage({ text: "Hello, my_application" });