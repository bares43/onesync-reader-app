/*global module*/

module.exports = function (grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    sass: {
      dist: {
        files: [{
          expand: true,
          cwd: 'Src/css/',
          src: ['*.scss'],
          dest: 'Build/',
          ext: '.css',
        }, ],
      },
    },
    uglify: {
      target: {
        files: {
          'Build/reader.js': ['Src/js/jquery-3.2.1.min.js', 'Src/js/hammer.min.js', 'Src/js/Base64.js', 'Src/js/reader.js'],
        },
      },
    },
    watch: {
      css: {
        files: ['Src/css/*'],
        tasks: ['build_css'],
      },
      js: {
        files: ['Src/js/*'],
        tasks: ['eslint', 'build_js'],
      },
    },
    eslint: {
      target: ['Src/js/*', '!Src/js/Base64.js', '!Src/js/jquery-3.2.1.min.js'],
    },
  });

  grunt.loadNpmTasks('grunt-contrib-sass');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-eslint');

  grunt.registerTask('build_css', ['sass']);
  grunt.registerTask('build_js', ['uglify']);
  grunt.registerTask('build', ['build_css', 'build_js']);
  grunt.registerTask('default', ['watch']);
};