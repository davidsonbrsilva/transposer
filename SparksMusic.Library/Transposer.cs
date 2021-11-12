﻿using SparksMusic.Library.Enum;
using SparksMusic.Library.Exceptions;
using SparksMusic.Library.Internal;
using System;
using System.Collections.Generic;

namespace SparksMusic.Library
{
    /// <summary>
    /// Transposer class
    /// </summary>
    public static class Transposer
    {
        private static readonly Node _flatMap = BuildFlatMap();
        private static readonly Node _sharpMap = BuildSharpMap();

        private const int SemitonesOnTheScale = 12;

        /// <summary>
        /// Transposes up a chord.
        /// </summary>
        /// <param name="chord">The chord</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord.</returns>
        /// <exception cref="ArgumentNullException">Thrown when chord object is null.</exception>
        /// <exception cref="NotAChordException">Thrown when input is not a valid chord.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static Chord TransposeUp(string chord, int semitones)
        {
            return TransposeUp(new Chord(chord), semitones);
        }

        /// <summary>
        /// Transposes up a chord.
        /// </summary>
        /// <param name="chord">The chord</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord.</returns>
        /// <exception cref="ArgumentNullException">Thrown when chord object is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static Chord TransposeUp(Chord chord, int semitones)
        {
            if (chord is null)
            {
                throw new ArgumentNullException(nameof(chord));
            }

            semitones = NormalizeSemitones(semitones);

            if (semitones == 0)
            {
                return chord;
            }

            var map = GetCorrectMap(_sharpMap, chord);

            var initialChordNode = FindHeadNodeFromNote(map, chord.Note);

            while (semitones > 0)
            {
                if (initialChordNode.Right != null)
                {
                    initialChordNode = initialChordNode.Right;
                    semitones--;
                }
                else
                {
                    initialChordNode = initialChordNode.Down;
                }
            }

            if (initialChordNode.Note.IsSharpOrDoubleSharp)
            {
                if (initialChordNode.Down != null)
                {
                    initialChordNode = initialChordNode.Down;
                }
            }

            return new Chord(initialChordNode.Note, chord.Tonality, chord.Complement, chord.Inversion);
        }

        /// <summary>
        /// Transposes up a list of chords.
        /// </summary>
        /// <param name="chords">The chord list</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord list</returns>
        /// <exception cref="ArgumentNullException">Thrown when chords object is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static List<Chord> TransposeUp(List<Chord> chords, int semitones)
        {
            if (chords is null)
            {
                throw new ArgumentNullException(nameof(chords));
            }

            var transposedChords = new List<Chord>();

            foreach (var chord in chords)
            {
                transposedChords.Add(TransposeUp(chord, semitones));
            }

            return transposedChords;
        }

        /// <summary>
        /// Transposes down a chord.
        /// </summary>
        /// <param name="chord">The chord</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord.</returns>
        /// <exception cref="ArgumentNullException">Thrown when chord object is null.</exception>
        /// <exception cref="NotAChordException">Thrown when input is not a valid chord.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static Chord TransposeDown(string chord, int semitones)
        {
            return TransposeDown(new Chord(chord), semitones);
        }

        /// <summary>
        /// Transposes down a chord.
        /// </summary>
        /// <param name="chord">The chord</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord.</returns>
        /// <exception cref="ArgumentNullException">Thrown when chords object is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static Chord TransposeDown(Chord chord, int semitones)
        {
            if (chord is null)
            {
                throw new ArgumentNullException(nameof(chord));
            }

            semitones = NormalizeSemitones(semitones);

            if (semitones == 0)
            {
                return chord;
            }

            var map = GetCorrectMap(_flatMap, chord);

            var initialChordNode = FindHeadNodeFromNote(map, chord.Note);

            while (semitones > 0)
            {
                if (initialChordNode.Left != null)
                {
                    initialChordNode = initialChordNode.Left;
                    semitones--;
                }
                else
                {
                    initialChordNode = initialChordNode.Up;
                }
            }

            if (initialChordNode.Note.IsFlatOrDoubleFlat)
            {
                if (initialChordNode.Up != null)
                {
                    initialChordNode = initialChordNode.Up;
                }
            }

            return new Chord(initialChordNode.Note, chord.Tonality, chord.Complement, chord.Inversion);
        }

        /// <summary>
        /// Transposes down a list of chords.
        /// </summary>
        /// <param name="chords">The chord list</param>
        /// <param name="semitones">The semitones to the transposition</param>
        /// <returns>A transposed chord list.</returns>
        /// <exception cref="ArgumentNullException">Thrown when chords object is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when semitones parameter is a negative number.</exception>
        public static List<Chord> TransposeDown(List<Chord> chords, int semitones)
        {
            if (chords is null)
            {
                throw new ArgumentNullException(nameof(chords));
            }

            var transposedChords = new List<Chord>();

            foreach (var chord in chords)
            {
                transposedChords.Add(TransposeDown(chord, semitones));
            }

            return transposedChords;
        }

        /// <summary>
        /// Get valid chord from a chords string list.
        /// </summary>
        /// <param name="chords">The chords</param>
        /// <returns>A list of valid chords.</returns>
        public static List<Chord> GetValidChords(List<string> chords)
        {
            var validChords = new List<Chord>();

            foreach (var chord in chords)
            {
                if (IsChord(chord))
                {
                    validChords.Add(new Chord(chord));
                }
            }

            return validChords;
        }

        /// <summary>
        /// Check if the given name is a valid chord.
        /// </summary>
        /// <param name="chordName">The chord name</param>
        /// <returns>True if the given name is a valid chord.</returns>
        public static bool IsChord(string chordName)
        {
            try
            {
                return new Chord(chordName) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static int NormalizeSemitones(int semitones)
        {
            if (semitones < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(semitones), "Semitones can not be negative numbers");
            }

            return semitones % SemitonesOnTheScale;
        }

        private static Node GetCorrectMap(Node map, Chord chord)
        {
            return chord switch
            {
                _ when chord.IsFlatOrDoubleFlat   => _flatMap,
                _ when chord.IsSharpOrDoubleSharp => _sharpMap,
                _                                 => map
            };
        }

        private static Node FindHeadNodeFromNote(Node mapHeadNode, Note note)
        {
            mapHeadNode.HasVisited = true;

            if (mapHeadNode.Note.Equals(note))
            {
                mapHeadNode.HasVisited = false;
                return mapHeadNode;
            }

            Node node = null;

            if (mapHeadNode.Right != null && !mapHeadNode.Right.HasVisited)
            {
                node = FindHeadNodeFromNote(mapHeadNode.Right, note);
            }

            if (node is null && mapHeadNode.Down != null && !mapHeadNode.Down.HasVisited)
            {
                node = FindHeadNodeFromNote(mapHeadNode.Down, note);
            }

            mapHeadNode.HasVisited = false;

            return node;
        }

        private static Node BuildFlatMap()
        {
            var head = new Node(new Note(NoteLetter.A, Accident.None));

            // A
            var current = head;
            current.Down = new Node(new Note(NoteLetter.B, Accident.DoubleFlat));
            current.Down.Up = current;

            // Bbb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.B, Accident.Flat));
            current.Right.Left = current;

            // Bb
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.B, Accident.None));
            current.Down = new Node(new Note(NoteLetter.C, Accident.DoubleFlat));
            current.Right.Left = current;
            current.Down.Up = current;

            // Cbb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.C, Accident.Flat));
            current.Right.Left = current;

            // Cb
            current = current.Right;
            current.Up = current.Left.Up.Right;
            current.Up.Down = current;
            current.Right = new Node(new Note(NoteLetter.C, Accident.None));
            current.Right.Left = current;

            // C
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.D, Accident.DoubleFlat));
            current.Down.Up = current;

            // Dbb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.D, Accident.Flat));
            current.Right.Left = current;

            // Db
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.D, Accident.None));
            current.Right.Left = current;

            // D
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.E, Accident.DoubleFlat));
            current.Down.Up = current;

            // Ebb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.E, Accident.Flat));
            current.Right.Left = current;

            // Eb
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.E, Accident.None));
            current.Right.Left = current;
            current.Down = new Node(new Note(NoteLetter.F, Accident.DoubleFlat));
            current.Down.Up = current;

            // Fbb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.F, Accident.Flat));
            current.Right.Left = current;

            // Fb
            current = current.Right;
            current.Up = current.Left.Up.Right;
            current.Up.Down = current;
            current.Right = new Node(new Note(NoteLetter.F, Accident.None));
            current.Right.Left = current;

            // F
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.G, Accident.DoubleFlat));
            current.Down.Up = current;

            // Gbb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.G, Accident.Flat));
            current.Right.Left = current;

            // Gb
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.G, Accident.None));
            current.Right.Left = current;

            // G
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.A, Accident.DoubleFlat));
            current.Down.Up = current;

            // Abb
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.A, Accident.Flat));
            current.Right.Left = current;

            // Ab
            current = current.Right;
            current.Right = head;
            current.Right.Left = current;

            return head;
        }

        private static Node BuildSharpMap()
        {
            var head = new Node(new Note(NoteLetter.A, Accident.None));

            // A
            var current = head;
            current.Right = new Node(new Note(NoteLetter.A, Accident.Sharp));
            current.Right.Left = current;

            // A#
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.A, Accident.DoubleSharp));
            current.Right.Left = current;

            // A##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.B, Accident.None));
            current.Down.Up = current;

            // B
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.B, Accident.Sharp));
            current.Right.Left = current;

            // B#
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.B, Accident.DoubleSharp));
            current.Right.Left = current;
            current.Down = new Node(new Note(NoteLetter.C, Accident.None));
            current.Down.Up = current;

            // B##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.C, Accident.Sharp));
            current.Down.Up = current;

            // C#
            current = current.Down;
            current.Left = current.Up.Left.Down;
            current.Left.Right = current;
            current.Right = new Node(new Note(NoteLetter.C, Accident.DoubleSharp));
            current.Right.Left = current;

            // C##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.D, Accident.None));
            current.Down.Up = current;

            // D
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.D, Accident.Sharp));
            current.Right.Left = current;

            // D#
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.D, Accident.DoubleSharp));
            current.Right.Left = current;

            // D##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.E, Accident.None));
            current.Down.Up = current;

            // E
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.E, Accident.Sharp));
            current.Right.Left = current;

            // E#
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.E, Accident.DoubleSharp));
            current.Right.Left = current;
            current.Down = new Node(new Note(NoteLetter.F, Accident.None));
            current.Down.Up = current;

            // E##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.F, Accident.Sharp));
            current.Down.Up = current;

            // F#
            current = current.Down;
            current.Left = current.Up.Left.Down;
            current.Left.Right = current;
            current.Right = new Node(new Note(NoteLetter.F, Accident.DoubleSharp));
            current.Right.Left = current;

            // F##
            current = current.Right;
            current.Down = new Node(new Note(NoteLetter.G, Accident.None));
            current.Down.Up = current;

            // G
            current = current.Down;
            current.Right = new Node(new Note(NoteLetter.G, Accident.Sharp));
            current.Right.Left = current;

            // G#
            current = current.Right;
            current.Right = new Node(new Note(NoteLetter.G, Accident.DoubleSharp));
            current.Right.Left = current;

            // G##
            current = current.Right;
            current.Down = head;
            current.Down.Up = current;

            return head;
        }
    }
}
