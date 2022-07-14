<script lang="ts">
    import type { TypeDefinition } from "./schema";
    import {
        isPrimitive,
        isNullable,
        isComplex,
    } from "../../../../../../client/sigger-gen/lib/sigger-enums";

    export let type: TypeDefinition | undefined;

    function fmtType() {
        if (!type) return "null";

        return type.exportedType;
    }
</script>

<pre><code>{fmtType()} </code></pre>

<style>
    pre {
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
        padding: 0.25rem;
    }
</style>
