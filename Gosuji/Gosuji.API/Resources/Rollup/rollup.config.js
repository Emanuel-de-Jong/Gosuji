import terser from '@rollup/plugin-terser';

const baseInputPath = "../js/";
const baseOutputPath = "../../../Gosuji.Client/wwwroot/js/"

const createPageBundle = (filePath, fileName = filePath) => ({
    input: baseInputPath + 'pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: baseOutputPath + 'pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: true
    },
    plugins: [
        terser({
            compress: {
                unused: false,       // Prevents removal of unused variables and functions
                side_effects: false  // Avoids dropping code that Terser thinks has no side effects
            },
        })
    ]
});

let config = [
    // Besogo minify
    {
        input: baseInputPath + 'libs/besogo.all.js',
        output: {
            file: baseOutputPath + 'libs/besogo.all.js',
            format: 'iife',
            sourcemap: true
        },
        plugins: [
            terser({
                compress: {
                    unused: false,
                    side_effects: false
                },
            })
        ]
    },

    // Custom globals bundle
    {
        input: baseInputPath + 'custom.js',
        output: {
            file: baseOutputPath + 'bundle.js',
            format: 'iife',
            sourcemap: true
        },
        plugins: [
            terser({
                compress: {
                    unused: false,
                    side_effects: false
                },
            })
        ]
    },

    // Custom globals bundle CMS
    {
        input: baseInputPath + 'custom.js',
        output: {
            file: "../../../Gosuji.CMS/wwwroot/js/bundle.js",
            format: 'iife',
            sourcemap: true
        }
    },

    // Page bundles
    createPageBundle('trainer'),
    createPageBundle('josekis'),
    createPageBundle('profile'),
];

export default config;
