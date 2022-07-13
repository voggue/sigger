<script lang="ts">
    import MethodDecl from "./MethodDecl.svelte";

    export let url: string;
    export let hub: any;

    function toggleView() {
        hub.visible = !hub.visible;
    }

    function toggleExpanded() {
        hub.expanded = !hub.expanded;
        if (hub.methods) {
            for (let method of hub.methods) {
                method.expanded = hub.expanded;
            }
        }
        if (hub.events) {
            for (let event of hub.events) {
                event.expanded = hub.expanded;
            }
        }
    }
</script>

<div class="heading">
    <h2>{hub.caption}</h2>

    <ul class="options">
        <li>
            <button on:click={toggleView}
                >{hub.visible ? "Hide" : "Show"}</button>
        </li>
        <li>
            <button on:click={toggleExpanded}
                >{hub.expanded ? "Collapse" : "Expand"} Operations</button>
        </li>
    </ul>
</div>

<ul class="endpoints">
    {#if hub?.methods?.length}
        {#each hub?.methods as method}
            <li class="endpoint">
                <MethodDecl {url} {hub} {method} />
            </li>
        {/each}
    {/if}
</ul>

<style>
    .heading {
        float: none;
        clear: both;
        display: block;
        border-width: 1px;
        border-style: solid;
        border-color: transparent;
        border-image: initial;
        overflow: hidden;
    }

    .heading h2 {
        color: rgb(153, 153, 153);
        padding-left: 0px;
        display: block;
        clear: none;
        float: left;
        font-family: "Droid Sans", sans-serif;
        font-weight: bold;
    }

    .heading ul.options {
        list-style: none;
        display: block;
        clear: none;
        float: right;
        overflow: hidden;
        padding: 0px;
        margin: 14px 10px 0px 0px;
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

    ul.endpoints {
        list-style: none;
        padding: 0;
        margin: 0;
    }
</style>
