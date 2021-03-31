# elsa-workflow
Workflow driven test application using Elsa and .NET Core

------------

Implementation based on [this tutorial](https://medium.com/@sipkeschoorstra/building-workflow-driven-net-core-applications-with-elsa-139523aa4c50 "this tutorial").

**Before running:**

- This implementation uses a mongoDB connection.  The connection URL can be configured in `appsettings.json`. Make sure to adjust it before running the application.

- Workflow designed for this example can be found inside `/Workflows` folder. After running the project, it is necessary first to import the workflow definition through Elsa's Dashboard interface. An example of this workflow is attached with the `json` file inside this same folder.

- The workflow is triggered after performing the `POST` operation on `/users/register`. Curl example:

`curl --location --request POST 'https://localhost:44306/users/register' \
--header 'Content-Type: application/json' \
--header 'Content-Type: text/plain' \
--data-raw '{
  "name": "user1",
  "email": "user1@mail.com",
  "password": "123",
  "repeatPassword": "123"
}'`

- Elsa's Dashboard runs on the same port the applications runs, and can be accessed under the path `/elsa/home`.

[Elsa's Project.](https://elsa-workflows.github.io/elsa-core/ "Elsa's Project.")
