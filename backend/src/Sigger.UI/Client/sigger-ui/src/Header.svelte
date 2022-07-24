<script lang="ts">
    import {onDestroy} from "svelte";

    import {url, fetchSchema, settings} from "./store";

    function isInvalidUrl() {
        if (!localUrl?.length) return false;
        try {
            new URL(localUrl);
            return false;
        } catch (_) {
            return true;
        }
    }

    let localUrl: string | null;

    const unsubscribe = url.subscribe((value) => {
        localUrl = value;
    });

    function tryConnect() {
        fetchSchema(localUrl);
    }

    onDestroy(unsubscribe);
</script>

<div id="header">
    <div class="sigger-ui-wrap">
        <a id="logo" href="https://github.com/voggue/sigger">
            <img
                    class="logo_img"
                    alt="sigger"
                    height="30"
                    width="30"
                    src="{($settings?.directory)}logo.png"
            />
            <span class="logo_title">Sigger</span>
        </a>
        <span id="api_selector">
      <div class="input">
        <input
                placeholder={localUrl}
                bind:value={localUrl}
                id="input_baseUrl"
                name="baseUrl"
                type="text"
        />
      </div>
      <div id="auth_container">
        
      </div>
      <div class="input">
        <button
                disabled={isInvalidUrl()}
                class="header_btn"
                on:click={tryConnect}
                data-sw-translate>Update Schema</button
        >
      </div>
    </span>
    </div>
</div>

<style>
    #header {
        background-color: #2c3e50;
        padding: 9px 14px 19px 14px;
        height: 23px;
    }

    .sigger-ui-wrap {
        line-height: 1;
        font-family: "Droid Sans", sans-serif;
        max-width: 960px;
        margin-left: auto;
        margin-right: auto;
    }

    #logo {
        font-size: 1.5em;
        font-weight: bold;
        text-decoration: none;
        color: white;
    }

    .logo_img {
        display: block;
        float: left;
        margin-top: 2px;
    }

    .logo_title {
        display: inline-block;
        padding: 5px 0 0 10px;
    }

    #api_selector {
        display: block;
        clear: none;
        float: right;
    }

    #api_selector .input {
        display: inline-block;
        clear: none;
        margin: 0 10px 0 0;
    }

    #api_selector .input input {
        font-size: 0.9em;
        padding: 3px;
        margin: 0;
        width: 400px;
    }

    #auth_container {
        color: #fff;
        display: inline-block;
        border: none;
        padding: 5px;
        height: 13px;
    }

    .header_btn {
        display: block;
        text-decoration: none;
        font-weight: bold;
        padding: 6px 8px;
        font-size: 0.9em;
        color: #2c3e50;
        background-color: #dadfe1;
    }

    .header_btn:disabled,
    .header_btn[disabled] {
        background-color: #ccc;
        color: #999;
    }
</style>
