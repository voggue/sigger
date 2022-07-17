import { writable } from "svelte/store";
import type { HubConnection } from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import type { SchemaDocument, HubDefinition } from "./schema";

export const error = writable<any | null>(null);
export const info = writable<any | null>(null);
export const url = writable<string | null>(null);
export const hubs = writable<HubWithMetadata[] | null>([]);
export const definition = writable<any | null>(null);
export const busy = writable(false);

export interface HubWithMetadata extends HubDefinition {
  connection?: HubConnection | null;
}

export async function fetchSchema(host: string) {
  try {

    const v = isValidUrl(host);
    url.update(() => v ? host : null);
    if (!v) return;

    console.log("get spec from " + host);
    error.update(() => null);
    hubs.update(() => null);
    busy.update(() => true);

    const resp = await fetch(host);
    const def: SchemaDocument | undefined = await resp.json();
    definition.update(() => def);

    const newHubs: HubWithMetadata[] = [];
    for (let hubIdx = 0; hubIdx < def.hubs.length; hubIdx++) {
      const hub = def.hubs[hubIdx];
      newHubs.push(hub)
    }
    hubs.update(() => newHubs);

  } catch (e) {
    error.update(() => e);
    console.error(e);
    return null;
  } finally {
    busy.update(() => false);
  }
}

export async function getConnection(hub: HubWithMetadata) {
  if (hub.connection) {
    return hub.connection;
  }

  try {
    console.log("connect to " + hub.caption ?? hub.name);
    const c = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7291/hubs/v1/chat", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        withCredentials: false
      })
      .configureLogging(signalR.LogLevel.Trace)
      .withAutomaticReconnect()
      .build();

    c.onclose(async () => {
      console.log("hub closed", c.connectionId)
    });

    c.onreconnected(async () => {
      console.log("hub reconnected", c.connectionId)
    })

    await c.start();

    hub.connection = c;
    console.log("hub connected ", c.connectionId)
    return hub.connection;

  } catch (e) {
    error.update(() => e);
    console.error(e);
    return null;
  }
}

function isValidUrl(host: string) {

  if (!host) return false;
  let u: URL | undefined;

  try {
    u = new URL(host);
  } catch (_) {
    return false;
  }

  return u.protocol === "http:" || u.protocol === "https:";
}
