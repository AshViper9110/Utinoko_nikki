<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class AnimalsController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $item = DB::table('animals')
                ->select('id', 'name')
                ->where('id', $id)
                ->first();

            return response()->json($item);
        }

        $items = DB::table('animals')
            ->select('id', 'name')
            ->get();

        return response()->json($items);
    }

    //登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255|unique:animals,name'
        ]);

        // データ登録
        $id = DB::table('animals')->insertGetId([
            'name' => $validated['name'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録した動物を取得
        $animal = DB::table('animals')
            ->select('id', 'name')
            ->where('id', $id)
            ->first();

        return response()->json($animal, 201);
    }
}
