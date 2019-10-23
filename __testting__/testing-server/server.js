const express = require("express");


// Create express app
let app = express();

// Serve some simple json
app.get("*", (_, res) => {
  res.send({
    test1: "one",
    test2: "two",
    test3: "three"
  });
});

const PORT = process.env.SWAGGER_DIFF_PORT;

app.listen(PORT, () => {
  console.log(`swagger-diff test server listening on ${PORT}`);
});

