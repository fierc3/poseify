// config.js
const env = process.env.NODE_ENV; // 'dev' or 'test'

const development = {
    backend: {
        baseUrl: "https://localhost:7236"
    }
};

const test = {
    backend: {
        baseUrl: "TBD"
    }
};

const production = {
    backend: {
        baseUrl: "TBD"
    }
};


const config = {
    development,
 test,production
};

const currentEnv = config[env]

export{currentEnv as env};