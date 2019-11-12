const express = require("express");
const path = require("path");

// Create express app
let app = express();

// Serve some simple json
app.get("/api/:serviceName/swagger/1/swagger.json", (req, res) => {

  // Get 1 or 2
  const sampleJsonNumber = Math.round(Math.random()) + 1;

  // Get service name 
  const { serviceName } = req.params;

  console.log(`Service File: ${serviceName}_${sampleJsonNumber}.json`);

  // Send test JSON file for service
  res.sendFile(path.join(__dirname, `./test-json/${serviceName}_${sampleJsonNumber}.json`));
});

const PORT = process.env.SWAGGER_DIFF_PORT;

app.listen(PORT, () => {
  console.log(`swagger-diff test server listening on ${PORT}`);
});

