![banner](banner.png)

# About

**The KSE Publications website is a collection of all major academic and analytical publications produced by the KSE community.**

For paper submissions, refer to the [Submissions page](https://publications.kse.ua/submissions).

Link to the website: https://publications.kse.ua

## Primary Features

- Simple yet very extensive search
- Article similarity calculation based on vectorization
- Analytics for publication views and search queries

# Tech Stack

- We use Notion for content management due to its friendliness and nice API.
- The Back-end is a .NET Application that pulls the content from Notion, stores it in Redis, and serves it via an API. Back-End also stores some usage analytics in a SQLite db.
- The Front-end is built using Astro, React, and shadcn/ui.

# Running

Running the Front-end locally should be very straightforward (you can do so with npm or Docker).

Running the Back-end requires a [Notion Integration](https://developers.notion.com/docs/getting-started#what-is-a-notion-integration) and a couple of [Notion databases](https://www.notion.so/help/what-is-a-database) -- Collections, Publications, Authors, and Publishers.
You can find a structure of those databases [here](https://clammy-hubcap-241.notion.site/KSE-Publications-Data-Structure-69b32eea48aa4aa193b5be8fe24e0b8d) (Click 'Duplicate' to save the structure to your Notion).

Make sure to specify the IDs of your databases inside `server/Publications/Publications.API/appsettings.json`.

Also, don't forget to create a `.env` file based on `env.example` for both `client` and `server`.

# The Team

- Mark Motliuk ([@marchellodev](https://github.com/marchellodev)) is the Technical Manager.
- Back-end development is led by Vlad Prudius ([@PrudiusVladislav](https://github.com/PrudiusVladislav)), and assisted by Anna Opanasenko ([@Pivdenna](https://github.com/Pivdenna)).
- Front-end development is led by Dmytro Kolisnyk ([@DmytroKolisnyk2](https://github.com/DmytroKolisnyk2)), and asisted by Tetiana Stasiuk ([@yeontannie](https://github.com/yeontannie)).

For the entire KSE Publications project team, refer to the [Team page](https://publications.kse.ua/team).

# Support

For any inquiries, please contact publications@kse.org.ua

# License

The code is published under the GNU General Public License version 3.
