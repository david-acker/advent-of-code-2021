(ns seven-segment-search
  (:require [clojure.string :as string])
  (:require [clojure.set :as set]))

(def file-name "C:\\Users\\david\\source\\repos\\advent-of-code-2021\\input\\day_8.txt")
(def input-data (string/split-lines (slurp file-name)))

;; Part One
(defn extract-values [index input]
  (-> input
      (string/split #"\|")
      (nth index)
      (string/trim)
      (string/split #" ")))

(defn get-part-one-result [input]
  (->> input
       (map (fn [x] (extract-values 1 x)))
       (flatten)
       (map count)
       (map (fn [x] (.contains [2, 3, 4, 7] x)))
       (filter true?)
       (count)))

(println (get-part-one-result input-data))

;; Part Two
(defn get-char-set [input]
  (->> input
       str
       char-array
       chars
       set))

(defn get-char-sets [index input]
  (->> input
       (extract-values index)
       (map get-char-set)))

;; Functions for decoding digits
(defn find-first [func coll]
  (first (filter func coll)))

(defn is-length [length coll]
  (= (count coll) length))

(defn find-one [input]
  (find-first (fn [x] (is-length 2 x)) input))

(defn find-four [input]
  (find-first (fn [x] (is-length 4 x)) input))

(defn find-seven [input]
  (find-first (fn [x] (is-length 3 x)) input))

(defn find-eight [input]
  (->> input
       (find-first (fn [x] (is-length 7 x)))))

(defn find-nine [input]
  (let [four    (find-four input)
        pred    (fn [x] (and (set/subset? four x) (is-length 6 x)))]
    (find-first pred input)))

(defn find-six [input]
  (let [one    (find-one input)
        pred   (fn [x] (and (not (set/subset? one x)) (is-length 6 x)))]
    (find-first pred input)))

(defn find-five [input]
  (let [six     (find-six input)
        pred    (fn [x] (and (set/subset? x six) (is-length 5 x)))]
    (find-first pred input)))

(defn find-three [input]
  (let [one      (find-one input)
        pred     (fn [x] (and (set/subset? one x) (is-length 5 x)))]
    (find-first pred input)))

(defn find-zero [input]
  (let [five    (find-five input)
        pred    (fn [x] (and (not (set/subset? five x)) (is-length 6 x)))]
    (find-first pred input)))

(defn find-two [input]
  (let [nine   (find-nine input)
        pred   (fn [x] (and (not (set/subset? x nine)) (is-length 5 x)))]
    (find-first pred input)))
    
(defn decode-digits [input]
  (let [decode-zero   #(assoc % 0 (find-zero input))
        decode-one    #(assoc % 1 (find-one input))
        decode-two    #(assoc % 2 (find-two input))
        decode-three  #(assoc % 3 (find-three input))
        decode-four   #(assoc % 4 (find-four input))
        decode-five   #(assoc % 5 (find-five input))
        decode-six    #(assoc % 6 (find-six input))
        decode-seven  #(assoc % 7 (find-seven input))
        decode-eight  #(assoc % 8 (find-eight input))
        decode-nine   #(assoc % 9 (find-nine input))]
    (-> (hash-map)
        (decode-zero)
        (decode-one)
        (decode-two)
        (decode-three)
        (decode-four)
        (decode-five)
        (decode-six)
        (decode-seven)
        (decode-eight)
        (decode-nine)
        (set/map-invert))))

(defn decode-output-value [input]
  (let [encoded-digits    (get-char-sets 0 input)
        encoded-output    (get-char-sets 1 input)
        decoded-digit-map (decode-digits encoded-digits)]
    (->> encoded-output
         (map decoded-digit-map)
         (string/join "")
         (Integer/parseInt))))

(defn get-part-two-result [input]
  (->> input
       (map decode-output-value)
       (reduce +)))

(println (get-part-two-result input-data))