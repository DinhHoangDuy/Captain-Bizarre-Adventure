#!/usr/bin/env python3

import os
from typing import List

def rename_files(directory: str, excluded_dirs: List[str] = None) -> None:
    """
    Recursively rename files by replacing spaces with underscores.
    
    Args:
        directory: Directory to scan for files with spaces
        excluded_dirs: List of directory names to exclude
    """
    if excluded_dirs is None:
        excluded_dirs = [".git", ".vs"]
    
    # Convert to absolute path
    directory = os.path.abspath(directory)
    print(f"Scanning directory: {directory}")
    
    rename_count = 0
    
    # Walk through all directories and files
    for root, dirs, files in os.walk(directory, topdown=True):
        # Modify dirs in-place to exclude certain directories
        dirs[:] = [d for d in dirs if d not in excluded_dirs]
        
        # Process files
        for filename in files:
            # Skip the script itself
            if filename == os.path.basename(__file__):
                continue
                
            if " " in filename:
                old_path = os.path.join(root, filename)
                new_filename = filename.replace(" ", "_")
                new_path = os.path.join(root, new_filename)
                
                try:
                    print(f"Renaming: {filename} → {new_filename}")
                    os.rename(old_path, new_path)
                    rename_count += 1
                except Exception as e:
                    print(f"Error renaming {filename}: {e}")
    
    print(f"\nRenaming complete! {rename_count} files renamed.")

if __name__ == "__main__":
    # Use the current directory (where the script is located)
    current_directory = os.path.dirname(os.path.abspath(__file__))
    
    # Run the rename operation
    rename_files(current_directory)
    
    # Keep terminal open to see results
    input("\nPress Enter to exit...")