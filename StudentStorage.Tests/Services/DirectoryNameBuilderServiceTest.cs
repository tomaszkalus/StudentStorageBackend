using StudentStorage.Services;

namespace StudentStorage.Tests.Services
{
    public class DirectoryNameBuilderServiceTest
    {
        [Fact]
        public void RemoveInvalidFilenameCharacters_ShouldRemoveInvalidCharacters()
        {
            // Arrange
            string filename = "file?name";
            string expected = "filename";

            // Act
            string result = DirectoryNameBuilderService.RemoveInvalidFilenameCharacters(filename);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RemoveInvalidFilenameCharacters_ShouldRemoveAllInvalidCharacters()
        {
            // Arrange
            string filename = "file<>:\"\\/|?*name";
            string expected = "filename";

            // Act
            string result = DirectoryNameBuilderService.RemoveInvalidFilenameCharacters(filename);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RemoveInvalidFilenameCharacters_ShouldNotRemoveValidCharacters()
        {
            // Arrange
            string filename = "filename";

            // Act
            string result = DirectoryNameBuilderService.RemoveInvalidFilenameCharacters(filename);

            // Assert
            Assert.Equal(filename, result);
        }

    }
}
