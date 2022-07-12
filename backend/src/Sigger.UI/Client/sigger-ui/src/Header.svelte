<script lang="ts">
    import { createEventDispatcher } from "svelte";

    const dispatch = createEventDispatcher();

    export let url: string;

    let changedUrl = "";
    function isInvalidUrl(item) {
        try {
            new URL(item);
            return false;
        } catch (_) {
            return true;
        }
    }

    function connect(event: Event) {
        dispatch("connect", {
            url: changedUrl,
        });
    }
</script>

<div id="header">
    <div class="sigger-ui-wrap">
        <a id="logo" href="https://github.com/voggue/sigger">
            <img
                class="logo_img"
                alt="sigger"
                height="30"
                width="30"
                src="./logo.png"
            />
            <span class="logo_title">Sigger</span>
        </a>
        <span id="api_selector">
            <div class="input">
                <input
                    placeholder={url}
                    bind:value={changedUrl}
                    id="input_baseUrl"
                    name="baseUrl"
                    type="text"
                />
            </div>
            <div id="auth_container" />
            <div class="input">
                <button
                    disabled={isInvalidUrl(changedUrl)}
                    class="header_btn"
                    on:click={connect}
                    data-sw-translate>Connect</button
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
        min-width: 775px;
    }

    .sigger-ui-wrap {
        line-height: 1;
        font-family: "Droid Sans", sans-serif;
        min-width: 760px;
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
        width: 87px;
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
