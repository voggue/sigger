'use strict';

const RESOURCE_TEMPLATE = `
<li class="resource">
    <div class="heading">
        <h2>
            <a href="#!/{{hub.exportedName}}" class="toggleEndpointList" data-id="{{hub.exportedName}}">{{hub.caption}}</a> 
            {{hub.details}}
        </h2>
        <ul class="options">
            <li>
                <a href="#!{{hub.exportedName}}" id="endpointListTogger_pet" class="toggleEndpointList" 
                    data-id="{{hub.exportedName}}" data-sw-translate="">Show/Hide</a>
            </li>
            <li>
                <a href="#" class="collapseResource" data-id="{{hub.exportedName}}" 
                data-sw-translate="">List Operations</a>
            </li>
            <li>
                <a href="#" class="expandResource" data-id="{{hub.exportedName}}" 
                data-sw-translate="">Expand Operations</a>
            </li>
        </ul> <!-- end of options -->
    </div> <!-- end of heading -->
    
    <ul class="endpoints" id="{{hub.exportedName}}_endpoint_list" >
    </ul> <!-- end of endpoints -->
</li>`;

const ENDPOINT_TEMPLATE = `
<li class="endpoint">  
    <ul class="operations">
        <li class="{{ep.type}} operation" id="{{hub.exportedName}}_{{ep.exportedName}}">
            <div class="heading">
                <h3>
                    <span class="http_method">
                        <a href="#!/{{hub.exportedName}}/{{ep.exportedName}}" class="toggleOperation">{{ep.type}}</a>
                    </span>
                    <span class="path">
                        <a href="#!/{{hub.exportedName}}/{{ep.exportedName}}" class="toggleOperation ">{{ep.exportedName}}</a>
                    </span>
                </h3>
                <ul class="options"
                    <li>
                        <a href="#!/{{hub.exportedName}}/{{ep.exportedName}}" class="toggleOperation">
                            <span class="markdown">
                                <p>{{ ep.caption }}</p>
                            </span>
                        </a>
                    </li>
                </ul> <!-- options -->
            </div> <!-- heading -->
          
            <div class="content" id="{{hub.exportedName}}_{{ep.exportedName}}_content"> <!-- todo: style="display: none" -->          
                <div class="authorize-wrapper authorize-wrapper_operation">
                    <div>
<!--                        <div class="authorize__btn authorize__btn_operation authorize__btn_operation_logout">-->
<!--                            <ul class="authorize-scopes">-->
<!--                                <li class="authorize__scope" title="read your pets">read:pets</li>-->
<!--                                <li class="authorize__scope" title="modify pets in your account">write:pets</li>-->
<!--                            </ul>-->
<!--                        </div>-->
                    </div>
                </div>
                
                <div class="response-class">
                    <h4><span data-sw-translate="">Response Class</span> (<span data-sw-translate="">Success</span>)</h4>
                    <div class="markdown"><p>successful operation</p></div>
  
                        <span class="model-signature">
                            <div>
                                <div>
                                    <ul class="signature-nav">
                                        <li><a class="description-link" href="#" data-sw-translate="">Model</a></li>
                                        <li><a class="snippet-link selected" href="#" data-sw-translate="">Example Value</a></li>
                                    </ul>
                                <div>
                                
                                <div class="signature-container">
                                    <div class="description" style="display: none;"> <!-- todo: style="display: none" -->   
                                    </div>
                                    
                                    <div class="snippet" style="display: block;">
                                        <div class="snippet_json" style="display: block;">
                                            <pre><code class="json">
                                                { TODO }
                                            </code></pre>
                                        </div> <!-- snippet_json -->
                                    <div> <!-- snippet -->
                                </div> <!-- signature-container -->
                            </div>  <!-- signature -->
                        </span> <!-- model-signature -->

                <!-- ## TODO -->
                </div> <!-- end of response-class -->
            </div> <!-- content -->
            
        </li> <!-- operation -->
    </ul> <!-- operations -->
</li> <!-- endpoint -->
`;

class SiggerUI {
    constructor(options) {
        if (!options) {
            this.options = {url: ""};
        } else if (typeof options === "string") {
            this.options = {url: options};
        } else {
            this.options = options;
        }
    }

    async updateUI() {
        try {
            const json = await this.fetch();
            if (!json) return;

            const siggerUrl = document.getElementById("sigger-url");
            if (siggerUrl) {
                siggerUrl.setAttribute("href", this.options.url);
                siggerUrl.innerText = this.options.url;
            } else {
                siggerUrl.innerText = "Sigger URL container not found";
            }

            const apiInfo = document.getElementById("api_info");
            if (apiInfo) {
                apiInfo.getElementsByClassName("info_title")[0].innerText = json.info.title;
                if (json.info.description)
                    apiInfo.getElementsByClassName("info_description")[0].innerText = json.info.description;
            } else {
                this.error("API info container not found");
            }

            const resources = document.getElementById("resources");
            if (resources) this.renderHubs(resources, json);
            else this.error("No resources container found");

        } catch (e) {
            this.error(e);
        }
    }

    async fetch() {
        try {
            const url = this.options.url;

            if (!url) {
                this.error("No url provided");
                return;
            }

            const resp = await fetch(url);
            return await resp.json();
        } catch (e) {
            this.error(e);
            return null;
        }
    }

    renderHubs(resources, json) {
        if (!json.hubs?.length) {
            this.error("No hubs found");
            return;
        }

        for (let hubIdx = 0; hubIdx < json.hubs.length; hubIdx++) {
            const hub = json.hubs[hubIdx];
            this.renderHub(resources, hub);
        }
    }

    renderHub(resources, hub) {

        resources.innerHTML += this.resolveTemplate(RESOURCE_TEMPLATE, {hub});

        // add methods
        const endpointContainer = document.getElementById(`${hub.exportedName}_endpoint_list`);
        for (let methodIdx = 0; methodIdx < hub.methods.length; methodIdx++) {
            const method = hub.methods[methodIdx];
            endpointContainer.innerHTML += this.resolveTemplate(ENDPOINT_TEMPLATE, {
                hub,
                ep: {...method, type: "method"}
            });
        }

        // add events
        for (let eventIdx = 0; eventIdx < hub.events.length; eventIdx++) {
            const event = hub.events[eventIdx];
            endpointContainer.innerHTML += this.resolveTemplate(ENDPOINT_TEMPLATE, {
                hub,
                ep: {...event, type: "event"}
            });
        }
    }

    error(error) {
        console.log(error);
        const msgBar = document.getElementById("message-bar");
        if (typeof error === "string") {
            msgBar.innerText = error;
        } else if (error.message) {
            msgBar.innerText = error.message;
        } else {
            msgBar.innerText = JSON.stringify(error);
        }
    }

    resolveTemplate(template, data) {
        function replacer(match, name, prop) {
            const obj = data[name];
            if (!obj) {
                console.log(`#ERR# object with name '${name}' not found`, data)
                return `#ERR# object with name '${name}' not found`;
            }
            const s = obj[prop];
            return s ? s : "";
        }

        const rx = new RegExp(`{{[ ]*([_a-zA-Z][^.]+).([_a-zA-Z][^}]+)[ ]*}}`, "g");
        return template.replace(rx, replacer);
    }
}

window.siggerUi = SiggerUI;