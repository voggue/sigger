<script lang="ts">
    import { fade } from "svelte/transition";

    export let url: string;
    export let hub: any;
    export let method: any;

    function toggleExpanded() {
        method.expanded = !method.expanded;
    }
</script>

<div class="heading">
    <h3>
        <span class="type">
            <a
                on:click={toggleExpanded}
                href="#!/{hub.exportedName}/{method.exportedName}">method</a
            >
        </span>
        <span class="path">
            <a
                on:click={toggleExpanded}
                href="#!/{hub.exportedName}/{method.exportedName}"
                >{method?.caption}</a
            >
        </span>
    </h3>

    <ul class="options">
        <li>
            <button on:click={toggleExpanded}
                >{method.expanded ? "Collapse" : "Expand"} Operations</button
            >
        </li>
    </ul>
</div>
{#if method.expanded}
    <div transition:fade class="content">
        <div class="respone">
            <h4>Response</h4>

            <div class="signature">
                <ul class="nav">
                    <li>
                        <a
                            class:active={method.showDescription}
                            href="#!/{hub.exportedName}/{method.exportedName}"
                            >Model</a
                        >
                    </li>
                    <li>
                        <a
                            class:active={!method.showDescription}
                            href="#!/{hub.exportedName}/{method.exportedName}"
                            >Example Value</a
                        >
                    </li>
                </ul>
                <div class="signature-container">
                    {#if method.showDescription}
                        <div class="description">
                            <!-- todo: style="display: none" -->
                        </div>
                    {:else}
                        <div class="snippet" style="display: block;">
                            <div class="json" style="display: block;">
                                <pre><code
                                        >x: true,
                                    y: false,
                                </code>
                                </pre>
                            </div>
                            <!-- snippet_json -->
                            <div><!-- snippet --></div>
                            <!-- signature-container -->
                        </div>
                    {/if}
                </div>
            </div>
        </div>
    </div>
{/if}

<style>
    .heading,
    .content {
        background-color: rgb(236, 240, 241);
        border: 1px solid rgb(218, 223, 225);
    }

    .heading {
        float: none;
        clear: both;
        display: block;
        overflow: hidden;
        margin: 0px;
        padding: 0px;
    }

    .heading a {
        font-weight: 100;
    }

    .heading h3 {
        display: block;
        clear: none;
        float: left;
        width: auto;
        line-height: 1.1em;
        color: black;
        margin: 0px;
        padding: 0px;
        font-size: 0.9rem;
    }

    .heading .type a {
        background-color: rgb(4, 147, 114);
        text-transform: uppercase;
        color: white;
        display: inline-block;
        min-width: 50px;
        text-align: center;
        text-decoration: none;
        padding: 7px 6px 4px 2px;
        border-radius: 2px;
        font-size: 0.75rem;
    }

    .heading .path a {
        color: black;
        text-decoration: none;
        padding: 0 0 0 0.5rem;
    }

    .heading ul.options {
        list-style: none;
        display: block;
        clear: none;
        float: right;
        overflow: hidden;
        padding: 0px;
        margin: 4px 10px 0px 0px;
        font-size: 0.9rem;
    }

    .heading ul.options li {
        display: block;
        clear: none;
        float: left;
        padding: 0px;
        margin: 0px;
        color: rgb(102, 102, 102);
    }

    .heading ul.options li:not(:last-child) {
        border-right: 1px solid rgb(221, 221, 221);
    }

    .options button {
        text-decoration: none;
        color: rgb(85, 85, 85);
        background: none;
        border: none;
    }

    .options button:hover {
        text-decoration: underline;
    }

    .content {
        padding: 0rem 1rem;
        margin-bottom: 1rem;
    }

    .respone h4 {
        display: block;
        margin-block-start: 1.33em;
        margin-block-end: 1.33em;
        margin-inline-start: 0px;
        margin-inline-end: 0px;
        font-weight: 100;
        color: rgb(4, 147, 114);
    }

    .respone .signature {
        font-size: 1em;
    }

    .signature .nav {
        display: inline-block;
        list-style: none;
        min-width: 230px;
        line-height: 1em;
        margin: 0px;
        padding: 0px;
    }

    .signature .nav li {
        float: left;
        margin: 0px 5px 0px 0px;
        padding: 2px 5px 0px 0px;
    }

    .signature .nav li:not(:last-child) {
        border-right: 1px solid rgb(221, 221, 221);
    }

    .nav li a {
        color: rgb(170, 170, 170);
        text-decoration: none;
        font-size: 0.9rem;
        font-weight: 100;
    }

    .nav li a.active {
        color: black;
    }

    .signature .signature-container {
        padding: 0;
        margin: 0;
    }

    .respone .signature .json pre {
        font-size: 0.85em;
        line-height: 1.2em;
        max-height: 200px;
        cursor: pointer;
        overflow: auto;
        background-color: rgb(255, 255, 255);
        border-width: 1px;
        border-style: solid;
        border-color: rgb(255, 255, 255);
        border-image: initial;
    }
</style>