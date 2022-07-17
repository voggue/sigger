<script lang="ts">
  import { fade } from "svelte/transition";
  import ParametersDecl from "./ParametersDecl.svelte";
  import { HubWithMetadata, getConnection } from "./store";

  export let hub: HubWithMetadata;
  export let eventDecl: any;

  const MAX_MESSAGES = 15;

  let selectedMessage: any;
  let lastMessage: { date: Date; data: any };
  let messages: { date: Date; data: any }[] = [];

  function toggleExpanded() {
    eventDecl.expanded = !eventDecl.expanded;
  }

  function convertResponse(args) {
    var fmt = {};
    for (let argIdx = 0; argIdx < eventDecl.arguments.length; argIdx++) {
      const argDef = eventDecl.arguments[argIdx];

      if (args.length > argIdx) {
        fmt[argDef.exportedName] = args[argIdx];
      } else {
        fmt[`arg${argIdx + 1}`] = args[argIdx];
      }
    }
    return fmt;
  }

  function formatMessage(message: any) {
    return JSON.stringify(message, undefined, 2);
  }

  function formatDate(d: Date) {
    let month = ("" + (d.getMonth() + 1)).padStart(2, "0");
    let day = ("" + d.getDate()).padStart(2, "0");
    let h = ("" + d.getHours()).padStart(2, "0");
    let m = ("" + "" + d.getMinutes()).padStart(2, "0");
    let s = ("" + "" + d.getSeconds()).padStart(2, "0");
    return `${day}.${month} ${h}:${m}:${s}`;
  }

  async function subscribeMethod() {
    eventDecl.subscribed = !eventDecl.subscribed;
    const connection = await getConnection(hub);
    if (eventDecl.subscribed) {
      connection.on(eventDecl.name, (...args) => {
        messages.push({ date: new Date(), data: convertResponse(args) });
        if (messages.length > MAX_MESSAGES) {
          messages.splice(0, messages.length - MAX_MESSAGES);
        }
        messages = [...messages];
        lastMessage = messages[messages.length - 1];
      });
    } else {
      connection.off(eventDecl.name);
    }
  }
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
        >{eventDecl.expanded ? "Collapse" : "Expand"} event</button
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

    <button class="btt-invoke" on:click={subscribeMethod}
      >{eventDecl.subscribed ? "Unsubscribe" : "Subscribe"}</button
    >
  </div>

  {#if eventDecl.subscribed}
    <div transition:fade class="subscription">
      <div class="section">
        <h4>Subscription</h4>
        <div class="subscription-container">
          <div class="subscription-messages">
            {#if messages?.length}
              {#each messages as message}
                <button on:click="{e => selectedMessage = message}">
                  {formatDate(message.date)}
                </button>
              {/each}
            {:else}
              <div class="no-messages">no messages received yet!</div>
            {/if}
          </div>
          <div class="subscription-content">
            {#if selectedMessage}
              <pre><code>{formatMessage(selectedMessage.data)}</code></pre>
            {:else if lastMessage}
              <pre><code>{formatMessage(lastMessage.data)}</code></pre>
            {/if}
          </div>
        </div>
      </div>
    </div>
  {/if}
{/if}

<style>
  .subscription-container {
    display: flex;
  }

  .subscription-messages {
    min-height: 100px;
    border-right: 1px solid rgb(245, 124, 0);
    background-color: rgb(255, 255, 255);
  }

  .subscription-content {
    min-height: 100px;
    flex-grow: 1;
  }

  pre {
    font-size: 0.85em;
    line-height: 1.2em;
    cursor: pointer;
    overflow: auto;
    background-color: rgb(255, 255, 255);
    border-width: 1px;
    border-style: solid;
    border-color: rgb(255, 255, 255);
    border-image: initial;
    padding: 0 0.25rem;
    height: 100%;
    margin: 0;
  }

  .no-messages {
    align-items: center;
    display: flex;
    width: 100%;
    justify-content: center;
    height: 100%;
    color: rgb(245, 124, 0);
    margin-right: 5rem;
  }

  .heading,
  .content,
  .subscription {
    background-color: rgba(252, 161, 48, 0.1);
    border: 1px solid rgba(252, 161, 48, 0.1);
  }

  .heading {
    margin-top: 1rem;
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
  }

  .section h4 {
    display: block;
    margin-block-start: 1.33em;
    margin-block-end: 1.33em;
    margin-inline-start: 0px;
    margin-inline-end: 0px;
    font-weight: 100;
    color: rgb(245, 124, 0);
  }

  .btt-invoke {
    padding: 5px;
    margin-top: 1rem;
    margin-bottom: 0.5rem;
  }

  .subscription-messages {
    display: flex;
    flex-direction: column;
    overflow: auto;
  }

  .subscription-messages button {
    border: none;
    background: none;
    border-bottom: 1px solid rgb(221, 221, 221);
    padding: 5px 15px;
  }
</style>
