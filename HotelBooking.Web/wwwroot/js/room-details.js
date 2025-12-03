// JavaScript functions for room details page

// Function to navigate to specific carousel slide
window.goToCarouselSlide = (carouselId, slideIndex) => {
    const carousel = document.getElementById(carouselId);
    if (carousel) {
        const carouselInstance = bootstrap.Carousel.getInstance(carousel) || new bootstrap.Carousel(carousel);
        carouselInstance.to(slideIndex);
    }
};

// Function to copy text to clipboard with fallback
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch (err) {
        // Fallback method
        const textArea = document.createElement('textarea');
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.select();
        try {
            document.execCommand('copy');
            return true;
        } catch (fallbackErr) {
            return false;
        } finally {
            document.body.removeChild(textArea);
        }
    }
};

// Initialize Bootstrap carousel with options
window.initializeRoomCarousel = (carouselId) => {
    const carousel = document.getElementById(carouselId);
    if (carousel) {
        new bootstrap.Carousel(carousel, {
            interval: false, // Disable auto-slide
            wrap: true,
            touch: true
        });
    }
};