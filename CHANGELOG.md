# Changelog

## [0.4.0] - 2025-10-02
### Added
- DirectionField and DijkstraField features.


## [0.3.0] - 2025-09-22
### Added
- Dijkstra map feature with tiles and diagonal movement weights parameters.

## [0.2.0] - 2025-09-16
### Added
- Diagonal movements support on pathfinding.

## [0.1.0] - 2025-09-12
### Added
- Full API unit tests.

### Removed
- All major order related structs, methods and parameter. Now only row major order is supported.

### Changed
- Optimize extraction and raycasting methods using circular algorithm (as circle and cones extraction and casting).

### Fixed
- Cone casting methods that was returning different result from cone extracting methods.
- Line algorythm returning same tiles multiple times in resulting array.
- Missing "includeWalls" parameter on some extraction methods.
- Direction angle not being calculated correctly on some methods.

## [0.0.3] - 2025-08-17
### Added
- Add GetTarget method on DirectionMap.

### Changed
- Remove int Target property on DirectionMap.
- Generic type constraints optimized on DirectionMap.

### Fixed
- Some cs doc comments issues.

## [0.0.2] - 2025-08-17
### Added
- Samples SharedAssets and APIPlayground.
- README file.
- CHANGELOG file.

### Changed
- Optimize DirectionMap generation.

### Fixed
- Some cs doc comments issues.