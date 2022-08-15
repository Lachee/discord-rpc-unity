
<table frame="void">
    <tr>
      <td width="200px">
        <img src="https://raw.githubusercontent.com/Lachee/discord-rpc-unity/master/Resources/discord_presence.png" align="center" width="100%" />
      </td>
      <td>
        <h1>Discord RPC Unity</h1>
        <p>
          This package provides a wrapper for [discord-rpc-csharp](https://github.com/lachee/discord-rpc-csharp) and
          a  better experience when intergrating with Unity3D, as well as solving some tricky annoyances such as named pipes and mono.
        </p>
      </td>
    </tr>
</table>

# Usage
Add the package to your project and look at the sample code. For more documentation about the RPC, check the 
[discord-rpc-csharp](https://github.com/lachee/discord-rpc-csharp) documentation

Check out the documentation at [https://lachee.github.io/discord-rpc-unity/](https://lachee.github.io/discord-rpc-unity/)

# Installation

<!--[![latest](https://github.com/Lachee/unity-utilities/actions/workflows/release.yml/badge.svg?branch=master)](https://github.com/Lachee/unity-utilities/actions/workflows/release.yml)-->

There are 3 methods you can use to importing this package:
1. You can download this as a zip and import it directly into unity
2. You can download the `.unitypackage` artifacts in the [latest build pre-release](https://github.com/Lachee/discord-rpc-unity/releases/tag/latest)
3. You can add the package to unity via Unity Package Manager, under the "add git package"

**Via the Package Manager wont notify you of updates, its Unity being dumb.**

## Manual Git Installation
This allows you to make changes and commit them back to the project:
```
git clone https://github.com/Lachee/discord-rpc-unity.git Packages/com.lachee.discordrpc
```
