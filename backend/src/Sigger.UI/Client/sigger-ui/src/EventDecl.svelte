<script lang="ts">
    import { fade } from "svelte/transition";
    import ParametersDecl from "./ParametersDecl.svelte";

    export let hub: any;
    export let eventDecl: any;

    function toggleExpanded() {
        eventDecl.expanded = !eventDecl.expanded;
    }

    function subscribeMethod() {}
</script>

<div class="heading">
    <h3>
        <span class="type">
            <a
                on:click={toggleExpanded}
                href="#!/{hub.exportedName}/{eventDecl.exportedName}">event</a
            >
        </span>
        <span class="path">
            <a
                on:click={toggleExpanded}
                href="#!/{hub.exportedName}/{eventDecl.exportedName}"
                >{eventDecl?.caption}</a
            >
        </span>
    </h3>

    <ul class="options">
        <li>
            <button on:click={toggleExpanded}
                >{eventDecl.expanded ? "Collapse" : "Expand"} Operations</button
            >
        </li>
    </ul>
</div>
{#if eventDecl.expanded}
    <div transition:fade class="content">
        {#if eventDecl.arguments?.length}
            <div class="section">
                <h4>Response</h4>

                <div class="parameters">
                    <ParametersDecl paramters={eventDecl.arguments} />
                </div>
            </div>
        {/if}

        <button class="btt-invoke" on:click={subscribeMethod}>Subscribe</button>
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
        background-color: rgb(245, 124, 0);
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

    .section h4 {
        display: block;
        margin-block-start: 1.33em;
        margin-block-end: 1.33em;
        margin-inline-start: 0px;
        margin-inline-end: 0px;
        font-weight: 100;
        color: rgb(4, 147, 114);
    }

    .section .signature {
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

    .btt-invoke {
        padding: 5px;
        margin-top: 1rem;
        margin-bottom: 0.5rem;
    }
</style>
