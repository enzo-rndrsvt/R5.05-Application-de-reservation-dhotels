// JavaScript functions for room details page with error handling

// Function to navigate to specific carousel slide
window.goToCarouselSlide = (carouselId, slideIndex) => {
    try {
        console.log(`?? goToCarouselSlide called: ${carouselId} ? slide ${slideIndex}`);
        
        const carousel = document.getElementById(carouselId);
        if (!carousel) {
            console.error(`? Carousel with ID ${carouselId} not found`);
            return;
        }

        // Méthode simple: changer les classes CSS directement
        const slides = carousel.querySelectorAll('.carousel-item');
        if (slideIndex >= 0 && slideIndex < slides.length) {
            // Supprimer la classe active de toutes les slides
            slides.forEach(slide => slide.classList.remove('active'));
            
            // Ajouter la classe active à la slide cible
            slides[slideIndex].classList.add('active');
            
            console.log(`? Successfully navigated to slide ${slideIndex}`);
            
            // Mettre à jour les indicateurs aussi
            updateIndicators(carousel, slideIndex);
        }
    } catch (error) {
        console.error('? Error in goToCarouselSlide:', error);
    }
};

// Function to update indicators
window.updateIndicators = (carousel, activeIndex) => {
    try {
        const indicators = carousel.querySelectorAll('.carousel-indicators button');
        indicators.forEach((indicator, index) => {
            if (index === activeIndex) {
                indicator.classList.add('active');
                indicator.setAttribute('aria-current', 'true');
            } else {
                indicator.classList.remove('active');
                indicator.setAttribute('aria-current', 'false');
            }
        });
    } catch (error) {
        console.error('Error updating indicators:', error);
    }
};

// Function to manually initialize a specific room carousel
window.initializeRoomCarousel = (carouselId) => {
    try {
        console.log(`?? Initializing room carousel: ${carouselId}`);
        
        const carousel = document.getElementById(carouselId);
        if (!carousel) {
            console.error(`? Carousel ${carouselId} not found for initialization`);
            return false;
        }

        // Marquer comme initialisé
        carousel._carouselInitialized = true;
        
        console.log(`? Room carousel ${carouselId} marked as initialized`);
        return true;
    } catch (error) {
        console.error(`? Error initializing room carousel ${carouselId}:`, error);
        return false;
    }
};

// Function to copy text to clipboard with fallback
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch (err) {
        // Fallback method
        try {
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            return true;
        } catch (fallbackErr) {
            console.error('Failed to copy to clipboard:', fallbackErr);
            return false;
        }
    }
};

// Initialize all room carousels on page load  
window.initializeCarousels = () => {
    try {
        console.log('?? Initializing all carousels...');
        
        // Trouver tous les carrousels
        document.querySelectorAll('[id^="roomCarousel_"], .carousel').forEach(carousel => {
            try {
                if (!carousel._carouselInitialized) {
                    carousel._carouselInitialized = true;
                    console.log(`? Initialized carousel: ${carousel.id || 'unnamed'}`);
                }
            } catch (error) {
                console.error(`? Failed to initialize carousel ${carousel.id || 'unnamed'}:`, error);
            }
        });
        
        console.log('?? Carousel initialization complete');
    } catch (error) {
        console.error('? Error in initializeCarousels:', error);
    }
};

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    console.log('?? DOM Content Loaded - initializing carousels');
    setTimeout(initializeCarousels, 100);
});

// Also initialize when Blazor components update
if (typeof document !== 'undefined') {
    document.addEventListener('blazor:enhanced:load', () => {
        console.log('?? Blazor enhanced load - initializing carousels');
        setTimeout(initializeCarousels, 200);
    });
}

// Blazor Server specific
if (typeof window !== 'undefined' && window.Blazor) {
    window.Blazor.addEventListener('enhancedload', () => {
        console.log('?? Blazor server enhanced load - initializing carousels');
        setTimeout(initializeCarousels, 200);
    });
}