<script lang="ts">
import Catalog from "./Catalog.svelte";

    import EventDecl from "./EventDecl.svelte";
    import MethodDecl from "./MethodDecl.svelte";
    import ModelDecl from "./ModelDecl.svelte";

    export let hub: any;

    let modelsExpanded = true;
    let catalogsExpanded = true;

    function toggleView() {
        hub.hidden = !hub.hidden;
    }

    function toggleExpanded() {
        hub.expanded = !hub.expanded;
        if (hub.expanded) {
            hub.hidden = false;
        }

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
            <button on:click={toggleView}>{hub.hidden ? "Show" : "Hide"}</button
            >
        </li>
        <li>
            <button on:click={toggleExpanded}
                >{hub.expanded ? "Collapse" : "Expand"} Operations</button
            >
        </li>
    </ul>
</div>

{#if !hub?.hidden}
    <ul class="endpoints">
        {#if hub?.methods?.length}
            {#each hub?.methods as method}
                <li class="endpoint">
                    <MethodDecl {hub} {method} />
                </li>
            {/each}
        {/if}

        {#if hub?.events?.length}
            {#each hub?.events as ev}
                <li class="endpoint">
                    <EventDecl {hub} eventDecl={ev} />
                </li>
            {/each}
        {/if}
    </ul>

    {#if hub?.definitions?.types?.length}
        <div class="models">
            <h4>
                <button on:click={() => (modelsExpanded = !modelsExpanded)}>
                    <span>Models</span>
                    {#if modelsExpanded}
                        <svg width="20" height="20" focusable="false">
                            <use xlink:href="#large-arrow-up" /></svg
                        >
                    {:else}
                        <svg width="20" height="20" focusable="false">
                            <use xlink:href="#large-arrow-down" /></svg
                        >
                    {/if}
                </button>
            </h4>

            {#if modelsExpanded}
                <ul class="models">
                    {#each hub.definitions.types as t}
                        <li class="model">
                            <ModelDecl {hub} typeDecl={t} />
                        </li>
                    {/each}
                </ul>
            {/if}
        </div>
    {/if}

    {#if hub?.definitions?.enums?.length}
        <div class="models">
            <h4>
                <button on:click={() => (catalogsExpanded = !catalogsExpanded)}>
                    <span>Catalogs</span>
                    {#if catalogsExpanded}
                        <svg width="20" height="20" focusable="false">
                            <use xlink:href="#large-arrow-up" /></svg
                        >
                    {:else}
                        <svg width="20" height="20" focusable="false">
                            <use xlink:href="#large-arrow-down" /></svg
                        >
                    {/if}
                </button>
            </h4>

            {#if catalogsExpanded}
                <ul class="models">
                    {#each hub.definitions.enums as e}
                        <li class="model">
                            <Catalog {hub} catalogDef={e} />
                        </li>
                    {/each}
                </ul>
            {/if}
        </div>
    {/if}

{:else}
    <hr />
{/if}

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

    ul.endpoints li {
        margin-bottom: 0.5rem;
    }

    div.models {
        border: 1px solid rgba(59, 65, 81, 0.3);
        border-radius: 4px;
        margin: 30px 0;
    }

    div.models h4 {
        align-items: center;
        color: #606060;
        cursor: pointer;
        display: flex;
        margin: 0;
        padding: 0.6rem 2rem 0.6rem 0.6rem;
        transition: all 0.2s;
        display: flex;
    }

    div.models h4 button {
        border: none;
        background-color: transparent;
        display: flex;
        flex-grow: 1;
        font-weight: 100;
        font-size: 1.2rem;
    }

    div.models h4 button span {
        flex-grow: 1;
        text-align: left;
    }

    ul.models {
        border-top: 1px solid rgba(59, 65, 81, 0.3);
        list-style: none;
        padding: 1rem;
    }
</style>
