const express = require("express");
const path = require("path");

// Create express app
let app = express();

// Serve some simple json
app.get("*", (_, res) => {
  res.sendFile(path.join(__dirname, "./test-json/admin-service.json"));
});

const PORT = process.env.SWAGGER_DIFF_PORT;

app.listen(PORT, () => {
  console.log(`swagger-diff test server listening on ${PORT}`);
});

